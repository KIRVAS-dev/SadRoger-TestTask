using Core.Gameplay.LocationBoundary;
using Core.Gameplay.Navigation;
using Core.Gameplay.Ship;
using Core.Loop;
using NUnit.Framework;

namespace Tests.EditMode
{
    internal sealed class LocationBoundaryServiceTests
    {
        private const float BoundaryRadius = 10f;
        private const float BoundaryWarningDelay = 1f;
        private const float BoundaryExitCountdown = 5f;
        private const float FrameTime = 0.1f;
        private const float Tolerance = 0.001f;

        private ShipModel _shipModel;
        private LocationBoundaryModel _model;
        private LocationBoundaryService _service;
        private int _exitCount;

        [SetUp]
        public void SetUp()
        {
            _shipModel = new ShipModel();
            _model = new LocationBoundaryModel();
            _service = new LocationBoundaryService(new FakeSettings(), new FakeCenterProvider(), _shipModel, _model);
            _exitCount = 0;
            _service.LocationExited += () => _exitCount++;
        }

        [Test]
        public void ShipInside_StaysInside()
        {
            Simulate(seconds: 10f);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Inside));
            Assert.That(_exitCount, Is.Zero);
        }

        [Test]
        public void ShipOutside_BeforeDelay_IsLeavingWithoutWarning()
        {
            MoveOutside();
            Simulate(seconds: 0.5f);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Leaving));
        }

        [Test]
        public void ShipOutside_AfterDelay_ShowsWarningWithCountdown()
        {
            MoveOutside();
            Simulate(seconds: 2f);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Warning));
            Assert.That(_model.CountdownRemaining.Value, Is.EqualTo(4f).Within(FrameTime + Tolerance));
        }

        [Test]
        public void ShipReturns_DuringWarning_CancelsTimer()
        {
            MoveOutside();
            Simulate(seconds: 3f);
            MoveInside();
            Simulate(seconds: 10f);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Inside));
            Assert.That(_model.CountdownRemaining.Value, Is.EqualTo(BoundaryExitCountdown));
            Assert.That(_exitCount, Is.Zero);
        }

        [Test]
        public void ShipStaysOutside_UntilCountdownEnds_RaisesExitOnce()
        {
            MoveOutside();
            Simulate(seconds: 20f);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Exited));
            Assert.That(_exitCount, Is.EqualTo(1));
        }

        [Test]
        public void ShipReturns_AfterExit_NextExitStartsNewCycle()
        {
            MoveOutside();
            Simulate(seconds: 7f);
            MoveInside();
            Simulate(FrameTime);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Inside));

            MoveOutside();
            Simulate(seconds: 3f);

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Warning));
            Assert.That(_exitCount, Is.EqualTo(1));

            Simulate(seconds: 4f);

            Assert.That(_exitCount, Is.EqualTo(2));
        }

        [Test]
        public void RepeatedShortExits_DoNotAccumulateTime()
        {
            for (int i = 0; i < 10; i++)
            {
                MoveOutside();
                Simulate(seconds: 3f);
                MoveInside();
                Simulate(FrameTime);
            }

            Assert.That(_model.State.Value, Is.EqualTo(LocationBoundaryState.Inside));
            Assert.That(_exitCount, Is.Zero);
        }

        private void MoveOutside()
        {
            _shipModel.Position.Value = new SeaPosition(BoundaryRadius + 1f, 0f);
        }

        private void MoveInside()
        {
            _shipModel.Position.Value = new SeaPosition(0f, 0f);
        }

        private void Simulate(float seconds)
        {
            int frameCount = (int)(seconds / FrameTime + Tolerance);

            for (int i = 0; i < frameCount; i++)
            {
                ((IGameplayTickable)_service).Tick(FrameTime);
            }
        }

        private sealed class FakeSettings : ILocationBoundarySettings
        {
            public float Radius => BoundaryRadius;
            public float WarningDelay => BoundaryWarningDelay;
            public float ExitCountdown => BoundaryExitCountdown;
        }

        private sealed class FakeCenterProvider : ILocationBoundaryCenterProvider
        {
            public SeaPosition Center => new SeaPosition(0f, 0f);
        }
    }
}
