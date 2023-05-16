using UnityEngine;

namespace Scripts.Pickups
{
    //Interface for Pickups
    public interface IPickups {
        public void OnPickupTriggerEnter(Collider2D col);
    }
}