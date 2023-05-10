using System;
using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletPierce : BulletBase {
        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, bounceLayer)) {
                Destroy(gameObject);
            }

            else {
                if (other.gameObject.TryGetComponent(out EnemyBase enemy)) {
                    enemy.TakeDamage(damage);
                }
            }
        }

        protected override void OnCollisionEnter2D(Collision2D other) {
            //neko arc
        }
    }
}