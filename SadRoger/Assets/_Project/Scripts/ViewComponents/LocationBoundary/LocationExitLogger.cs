using Core.Gameplay.LocationBoundary;
using Core.Lifecycle;
using UnityEngine;

namespace ViewComponents.LocationBoundary
{
    public sealed class LocationExitLogger : ISubscriptionLifecycle
    {
        private const string ExitMessage = "Location exited";

        private readonly ILocationBoundaryEvents _events;

        public LocationExitLogger(ILocationBoundaryEvents events)
        {
            _events = events;
        }

        void ISubscriptionLifecycle.Start()
        {
            _events.LocationExited += OnLocationExited;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _events.LocationExited -= OnLocationExited;
        }

        private static void OnLocationExited()
        {
            Debug.Log(ExitMessage);
        }
    }
}
