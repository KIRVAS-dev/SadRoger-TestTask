using System.Collections.Generic;
using ContentValidation;
using Core.Lifecycle;
using VContainer.Internal;

namespace Core.Bootstrap
{
    public sealed class ScopeLifecycle
    {
        private readonly IReadOnlyList<IValidatable> _validatables;
        private readonly IReadOnlyList<IWarmupLifecycle> _warmupLifecycles;
        private readonly IReadOnlyList<ISubscriptionLifecycle> _subscriptionLifecycles;

        public ScopeLifecycle(
            ContainerLocal<IReadOnlyList<IValidatable>> validatables,
            ContainerLocal<IReadOnlyList<IWarmupLifecycle>> warmupLifecycles,
            ContainerLocal<IReadOnlyList<ISubscriptionLifecycle>> subscriptionLifecycles)
        {
            _validatables = validatables.Value;
            _warmupLifecycles = warmupLifecycles.Value;
            _subscriptionLifecycles = subscriptionLifecycles.Value;
        }

        public void Start()
        {
            Validate();
            Warmup();
            StartSubscriptions();
        }

        public void Stop()
        {
            foreach (ISubscriptionLifecycle subscriptionLifecycle in _subscriptionLifecycles)
            {
                subscriptionLifecycle.Stop();
            }
        }

        private void Validate()
        {
            foreach (IValidatable validatable in _validatables)
            {
                validatable.Validate();
            }
        }

        private void Warmup()
        {
            foreach (IWarmupLifecycle warmupLifecycle in _warmupLifecycles)
            {
                warmupLifecycle.Warmup();
            }
        }

        private void StartSubscriptions()
        {
            foreach (ISubscriptionLifecycle subscriptionLifecycle in _subscriptionLifecycles)
            {
                subscriptionLifecycle.Start();
            }
        }
    }
}
