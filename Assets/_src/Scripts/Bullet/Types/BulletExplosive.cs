using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletExplosive : BulletBase {

        [Header("Config")] 
        [SerializeField] private float radius;
        [SerializeField] private float splashDamage; 
        
        protected override void OnCollisionEnter2D(Collision2D col) {
            if (!CheckLayerMask.IsInLayerMask(col.gameObject, destroyLayer)) return;
            
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var obj in colliders) {
                if (obj.TryGetComponent(out EnemyBase enemy)) {
                    var dist = Vector3.Distance(enemy.transform.position, transform.position);
                    enemy.TakeDamage(Mathf.Lerp(splashDamage, 0, dist/radius));
                }
            }
            
            Destroy(gameObject);
        }
    }
}