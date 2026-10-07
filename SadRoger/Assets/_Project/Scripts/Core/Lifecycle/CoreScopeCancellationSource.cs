using System;
using System.Threading;

namespace Core.Lifecycle
{
    internal sealed class CoreScopeCancellationSource
        : ICoreScopeCancellation,
          IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        CancellationToken ICoreScopeCancellation.Token => _cancellationTokenSource.Token;

        void IDisposable.Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
