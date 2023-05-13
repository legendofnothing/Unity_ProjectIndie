using _src.Scripts.Core;
using _src.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletExplosive : BulletBase {

        [Header("Config")] 
        [SerializeField] private float radius;
        [SerializeField] private float splashDamage;

        protected override void OnBounce(GameObject hitGameObject) {
            CanMove = false;
            Animator.SetTrigger("Explode");
            transform.DOScale(new Vector3(radius, radius), 0.4f);
            Player.DoCameraShake(0.32f, 1.2f);

            var hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
            foreach (var obj in hits) {
                if (!obj.TryGetComponent(out EnemyBase enemy)) continue;
                
                var dist = Vector3.Distance(obj.transform.position, transform.position);
                var desiredDamage = Mathf.Lerp(splashDamage, 0, dist / radius);
                
                if (desiredDamage < 1f) continue; 
                enemy.TakeDamage(desiredDamage);
            }
        }

        public void DestroyOnAnimationFinish() {
            OnDestroy();
        }
    }
}