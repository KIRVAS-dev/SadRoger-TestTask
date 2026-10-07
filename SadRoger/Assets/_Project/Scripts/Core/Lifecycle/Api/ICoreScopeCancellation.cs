using System.Threading;

namespace Core.Lifecycle
{
    internal interface ICoreScopeCancellation
    {
        CancellationToken Token { get; }
    }
}
