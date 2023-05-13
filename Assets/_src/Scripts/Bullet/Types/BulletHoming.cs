using System;
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
        [SerializeField] private Ease switchTargetEaseType;
        [SerializeField] private float radius;
        [SerializeField] private float splashDamage;

        private Tween _currentTween;
        private EnemyBase _currentEnemy;
        private bool _canSetNewTarget;
        
        protected override void OnSpawn() {
            CanMove = false;
            var closestEnemy = EnemyManager.instance.enemies
                .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            _currentEnemy = closestEnemy;
            _canSetNewTarget = true;
            
            if (closestEnemy != null) 
                _currentTween = transform.DOMove(closestEnemy.transform.position, 1 / speed).SetEase(easeType)
                    .OnComplete(ExplodeLogic);
        }

        protected void Update() {
            if (_canSetNewTarget && (_currentEnemy.isEnemyDying || _currentEnemy == null)) {
                _canSetNewTarget = false;
                OnTargetLost();
            }
        }

        public void DestroyOnAnimationFinish() {
            _currentTween.Kill();
            OnDestroy();
        }
        
        private void OnTargetLost() {
            _currentTween.Kill();
            var closestEnemy = EnemyManager.instance.enemies
                .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closestEnemy != null) {
                _currentEnemy = closestEnemy;
                _canSetNewTarget = true;
                _currentTween = transform.DOMove(closestEnemy.transform.position, 1 / speed).SetEase(switchTargetEaseType)
                    .OnUpdate(() => {
                        if (closestEnemy.isEnemyDying) OnTargetLost();
                    })
                    .OnComplete(ExplodeLogic);   
            }
        }

        private void ExplodeLogic() {
            Animator.SetTrigger("Explode");
            transform.DOScale(new Vector3(1f, 1f), 0.4f);
            Player.DoCameraShake(0.32f, 1.2f);
            var hits = new Collider2D[10];
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hits);
        
            for (var i = 0; i < size; i++) {
                var obj = hits[i];
                if (!obj.TryGetComponent(out EnemyBase enemy)) continue;
                var dist = Vector3.Distance(enemy.transform.position, transform.position);
                var desiredDamage = Mathf.Lerp(splashDamage, 0, dist / radius);
                if (desiredDamage < 1f) continue; 
                enemy.TakeDamage(desiredDamage);
            }
        }
    }
}