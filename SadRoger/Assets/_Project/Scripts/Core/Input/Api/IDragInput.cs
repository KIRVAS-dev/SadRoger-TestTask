using System;

namespace Core.Input
{
    public interface IDragInput
    {
        event Action<float> DragNormalizedDeltaChanged;
    }
}
