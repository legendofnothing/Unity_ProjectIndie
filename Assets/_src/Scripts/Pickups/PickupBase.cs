using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Pickups
{
    public class PickupBase : MonoBehaviour
    {
        public LayerMask bulletLayer;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer))
            {
                this.SendMessage(EventType.AddBullet);
                Destroy(gameObject);
            }
        }
    }
}
