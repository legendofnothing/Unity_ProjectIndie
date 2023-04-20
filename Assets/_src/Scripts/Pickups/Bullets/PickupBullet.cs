using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Pickups.Bullets
{
    public class PickupBullet : PickupBase
    {
        public GameObject bullet;
        public override void OnPickupTriggerEnter(Collider2D col) {
            if (!CheckLayerMask.IsInLayerMask(col.gameObject, pickupLayer)) return;
            Player.Player.instance.bulletManager.AddBullet(bullet);
            Destroy(gameObject);
        }
    }
}