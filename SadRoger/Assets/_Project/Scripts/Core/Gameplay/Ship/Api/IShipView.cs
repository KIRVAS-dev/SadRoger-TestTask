namespace Core.Gameplay.Ship
{
    public interface IShipView
    {
        void SetPosition(float x, float z);
        void SetHeading(float heading);
    }
}
