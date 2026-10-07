using Cysharp.Threading.Tasks;
using System.Threading;

namespace Infrastructure.Audio
{
    public interface IAudioLoader
    {
        UniTask LoadAsync(CancellationToken cancellationToken);
    }
}
