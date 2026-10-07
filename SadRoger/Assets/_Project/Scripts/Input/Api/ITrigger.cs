using System;

namespace Input
{
    public interface ITrigger
    {
        event Action Triggered;
    }
}
