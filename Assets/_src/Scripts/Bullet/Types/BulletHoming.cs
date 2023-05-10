using System.Linq;
using _src.Scripts.Core;
using _src.Scripts.Enemy;
using _src.Scripts.Managers;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletHoming : BulletBase {
        [Header("Bullet Config")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float radius;
        [SerializeField] private float splashDamage;

        private Tween _currentTween;
        
        protected override void OnSpawn() {
            var closestEnemy = EnemyManager.instance.enemies
                .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closestEnemy != null) 
                _currentTween = transform.DOMove(closestEnemy.transform.position, 1 / speed).SetEase(easeType)
                        .OnUpdate(() => { if (closestEnemy == null) OnTargetLost(); })
                        .OnComplete(() => {
                            var hits = new Collider2D[10];
                            var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hits);

                            for (var i = 0; i < size; i++) {
                                var obj = hits[i];
                                if (!obj.TryGetComponent(out EnemyBase enemy)) continue;
                                var dist = Vector3.Distance(enemy.transform.position, transform.position);
                                enemy.TakeDamage(Mathf.Lerp(splashDamage, 0, dist/radius));
                            }
                            Destroy(gameObject);
                        })
            ;
        }

        protected override void OnCollisionEnter2D(Collision2D other) {
            //
        }

        private void OnTargetLost() {
            _currentTween.Pause();
            var closestEnemy = EnemyManager.instance.enemies
                .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closestEnemy != null)
                _currentTween = transform.DOMove(closestEnemy.transform.position, 1 / speed).SetEase(easeType)
                    .OnUpdate(() => {
                        if (closestEnemy.isEnemyDying) OnTargetLost();
                    });
        }
    }
}