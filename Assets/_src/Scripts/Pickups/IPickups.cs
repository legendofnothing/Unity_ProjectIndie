using UnityEngine;

namespace _src.Scripts.Pickups
{
    //Interface for Pickups
    public interface IPickups {
        public void OnPickupTriggerEnter(Collider2D col);
    }
}