using System;
using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.Pickups
{
    public abstract class PickupBase : MonoBehaviour, IPickups
    {

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Bullet"))
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
