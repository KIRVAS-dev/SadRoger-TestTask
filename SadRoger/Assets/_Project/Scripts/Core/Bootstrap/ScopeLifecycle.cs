using System.Collections.Generic;
using ContentValidation;
using Core.Lifecycle;
using VContainer.Internal;

namespace Core.Bootstrap
{
    public sealed class ScopeLifecycle
    {
        private readonly IReadOnlyList<IValidatable> _validatables;
        private readonly IReadOnlyList<IPreparationLifecycle> _preparationLifecycles;
        private readonly IReadOnlyList<ISubscriptionLifecycle> _subscriptionLifecycles;

        public ScopeLifecycle(
            ContainerLocal<IReadOnlyList<IValidatable>> validatables,
            ContainerLocal<IReadOnlyList<IPreparationLifecycle>> preparationLifecycles,
            ContainerLocal<IReadOnlyList<ISubscriptionLifecycle>> subscriptionLifecycles)
        {
            _validatables = validatables.Value;
            _preparationLifecycles = preparationLifecycles.Value;
            _subscriptionLifecycles = subscriptionLifecycles.Value;
        }

        public void Start()
        {
            Validate();
            Prepare();
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

        private void Prepare()
        {
            foreach (IPreparationLifecycle preparationLifecycle in _preparationLifecycles)
            {
                preparationLifecycle.Prepare();
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
