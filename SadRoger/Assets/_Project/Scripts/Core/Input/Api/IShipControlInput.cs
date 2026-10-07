using System;

namespace Core.Input
{
    public interface IShipControlInput
    {
        event Action ControlChanged;

        float Throttle { get; }
        float Steering { get; }
    }
}
