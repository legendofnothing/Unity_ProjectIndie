using System;
using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletPierce : BulletBase {
        protected override void FixedUpdate() {
            transform.position += Dir * (speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, enemyLayer)) {
                other.gameObject.GetComponent<EnemyBase>().TakeDamage(damage);
            }
            
            if (CheckLayerMask.IsInLayerMask(other.gameObject, destroyLayer)) {
                OnBulletDestroy();
            }
        }
    }
}