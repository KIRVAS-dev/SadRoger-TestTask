using Core.Gameplay.Navigation;
using NUnit.Framework;

namespace Tests.EditMode
{
    internal sealed class AngleHelperTests
    {
        private const float Tolerance = 0.001f;

        [TestCase(0f, 0f)]
        [TestCase(360f, 0f)]
        [TestCase(720f, 0f)]
        [TestCase(-90f, 270f)]
        [TestCase(450f, 90f)]
        public void NormalizeDegrees_WrapsIntoZeroToFullTurn(float degrees, float expected)
        {
            Assert.That(AngleHelper.NormalizeDegrees(degrees), Is.EqualTo(expected).Within(Tolerance));
        }

        [TestCase(0f, 90f, 90f)]
        [TestCase(0f, 270f, -90f)]
        [TestCase(350f, 10f, 20f)]
        [TestCase(10f, 350f, -20f)]
        [TestCase(0f, 180f, 180f)]
        [TestCase(90f, 90f, 0f)]
        public void SignedDeltaDegrees_ReturnsShortestSignedTurn(
            float from,
            float to,
            float expected)
        {
            Assert.That(AngleHelper.SignedDeltaDegrees(from, to), Is.EqualTo(expected).Within(Tolerance));
        }
    }
}
