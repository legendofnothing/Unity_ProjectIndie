using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Pickups.Bullets
{
    public class PickupBulletCommon : PickupBase
    {
        public GameObject bullet;
        
        public override void PickupBehavior()
        {
            this.SendMessage(EventType.AddBullet, bullet);
        }
    }
}