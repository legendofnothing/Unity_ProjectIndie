using System;
using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletPierce : BulletBase {
        protected override void OnCollisionEnter2D(Collision2D col) {
            //bruynyur
        }

        protected void OnTriggerEnter2D(Collider2D other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, bounceLayer)) {
                other.GetComponent<EnemyBase>().TakeDamage(damage);
            }

            if (CheckLayerMask.IsInLayerMask(other.gameObject, destroyLayer)) {
                Destroy(gameObject);
            }
        }
    }
}