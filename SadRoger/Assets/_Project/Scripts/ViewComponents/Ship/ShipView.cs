using Core.Gameplay.Ship;
using UnityEngine;

namespace ViewComponents.Ship
{
    [DisallowMultipleComponent]
    public sealed class ShipView
        : MonoBehaviour,
          IShipView
    {
        void IShipView.SetPosition(float x, float z)
        {
            transform.position = new Vector3(x, transform.position.y, z);
        }

        void IShipView.SetHeading(float heading)
        {
            transform.rotation = Quaternion.Euler(0f, heading, 0f);
        }
    }
}
