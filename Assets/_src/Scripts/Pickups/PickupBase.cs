using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.Pickups
{
    public abstract class PickupBase : MonoBehaviour, IPickups
    {
        public LayerMask bulletLayer;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer))
            {
                PickupBehavior();
                Destroy(gameObject);
            }
        }

        public virtual void PickupBehavior()
        {
            //Implement override in child class
        }
    }
}
