using System;
using System.Collections;
using DG.Tweening;
using Enemy;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.Bullet.Types {
    public class BulletHoming : BulletBase {
        [Header("Bullet Config")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float radius;
        [SerializeField] private float splashDamage;

        private Tween _currentTween;

        protected override void OnSpawn() {
            CanMove = false;
        }

        protected override void FixedUpdate() {
            //brynuy
        }

        public void SetDestination(Vector3 destination) {
            _currentTween = transform.DOMove(destination, 1 / speed)
                .SetEase(easeType)
                .OnComplete(ExplodeLogic);
        }

        public void StopTracking() {
            _currentTween.Pause();
        }

        public IEnumerator SelfDestruct() {
            transform.DOScale(new Vector3(0.4f, 0.4f), 0.2f);
            yield return new WaitForSeconds(1.2f);
            Animator.SetTrigger("Explode");
        }

        public void DestroyOnAnimationFinish() {
            _currentTween.Kill();
            OnBulletDestroy();
        }
        
        private void ExplodeLogic() {
            EventDispatcher.instance.SendMessage(EventType.TargetSystemOnTargetHit, this);
            transform.DOScale(new Vector3(radius, radius), 0.4f);
            Animator.SetTrigger("Explode");

            var hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
            foreach (var obj in hits) {
                if (!obj.TryGetComponent(out EnemyBase enemy)) continue;
                
                var dist = Vector3.Distance(obj.transform.position, transform.position);
                var desiredDamage = Mathf.Lerp(splashDamage, 0, dist / radius);
                
                if (desiredDamage < 1f) continue; 
                enemy.TakeDamage((float) Math.Round(desiredDamage, 1));
            }
        }
    }
}