using UnityEngine;

namespace _src.Scripts.Pickups
{
    public abstract class PickupBase : MonoBehaviour, IPickups {
        public LayerMask pickupLayer; 
        private void OnTriggerEnter2D(Collider2D col) => OnPickupTriggerEnter(col);
        public virtual void OnPickupTriggerEnter(Collider2D col) { }
    }
}
