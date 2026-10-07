using System;
using Core.Gameplay.Ship;
using Core.Gameplay.Wind;
using Infrastructure.ExtendedExceptions;
using NUnit.Framework;

namespace Tests.EditMode
{
    internal sealed class WindTests
    {
        private const float Tolerance = 0.001f;

        private WindModel _windModel;
        private ShipModel _shipModel;
        private WindService _windService;

        [SetUp]
        public void SetUp()
        {
            _windModel = new WindModel();
            _shipModel = new ShipModel();
            _windService = new WindService(_windModel);
        }

        [Test]
        public void SetDirection_NormalizesDirection()
        {
            ((IWindService)_windService).SetDirection(370f);

            Assert.That(_windModel.Direction.Value, Is.EqualTo(10f).Within(Tolerance));
        }

        [TestCase(-0.1f)]
        [TestCase(1.1f)]
        public void SetStrength_OutOfRange_Throws(float strength)
        {
            Assert.That(() => ((IWindService)_windService).SetStrength(strength), Throws.InstanceOf<ExtendedException>());
        }

        [Test]
        public void SetMultiplier_ScalesAndClampsStrength()
        {
            ((IWindService)_windService).SetStrength(0.4f);
            ((IWindStrengthMultiplier)_windService).SetMultiplier(0.5f);

            Assert.That(_windModel.BaseStrength.Value, Is.EqualTo(0.4f).Within(Tolerance));
            Assert.That(_windModel.Strength.Value, Is.EqualTo(0.2f).Within(Tolerance));

            ((IWindStrengthMultiplier)_windService).SetMultiplier(5f);

            Assert.That(_windModel.Strength.Value, Is.EqualTo(1f).Within(Tolerance));
        }

        [TestCase(90f, 0f, 90f)]
        [TestCase(90f, 90f, 0f)]
        [TestCase(0f, 90f, -90f)]
        [TestCase(270f, 0f, -90f)]
        [TestCase(180f, 0f, 180f)]
        public void RelativeAngle_FollowsWindAndShipHeading(
            float windDirection,
            float shipHeading,
            float expected)
        {
            RelativeWindAngle relativeWindAngle = new RelativeWindAngle(_windModel, _shipModel);

            ((IWindService)_windService).SetDirection(windDirection);
            _shipModel.Heading.Value = shipHeading;

            Assert.That(((IRelativeWindAngle)relativeWindAngle).Angle.CurrentValue, Is.EqualTo(expected).Within(Tolerance));

            ((IDisposable)relativeWindAngle).Dispose();
        }
    }
}
