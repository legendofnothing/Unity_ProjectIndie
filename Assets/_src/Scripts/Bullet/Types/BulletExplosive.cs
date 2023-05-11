using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletExplosive : BulletBase {

        [Header("Config")] 
        [SerializeField] private float radius;
        [SerializeField] private float splashDamage;

        protected override void OnBounce() {
            _player.DoCameraShake(0.32f, 1.2f);
            var hits = new Collider2D[10];
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hits);

            for (var i = 0; i < size; i++) {
                var obj = hits[i];
                if (!obj.TryGetComponent(out EnemyBase enemy)) continue;
                var dist = Vector3.Distance(enemy.transform.position, transform.position);
                enemy.TakeDamage(Mathf.Lerp(splashDamage, 0, dist/radius));
            }

            Destroy(gameObject);
        }
    }
}