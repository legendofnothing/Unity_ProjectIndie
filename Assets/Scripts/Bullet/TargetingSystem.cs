using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Bullet.Types;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using Scripts.Enemy;
using Scripts.Managers;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.Bullet {
    public class TargetingSystem : Singleton<TargetingSystem> {
        private EnemyManager _enemyManager;
        private Dictionary<BulletHoming, EnemyBase> _targetList = new(); 

        private void Start() {
            _enemyManager = EnemyManager.instance;
            
            EventDispatcher.instance.SubscribeListener(EventType.TargetSystemOnTargetHit
                , bullet => HandleOnTargetHit((BulletHoming) bullet));
            
            EventDispatcher.instance.SubscribeListener(EventType.OnEnemyDying
                , enemy => HandleOnTargetDie((EnemyBase) enemy));
        }

        public void AddHomingBullet(BulletHoming recipient) {
            _targetList.Add(recipient, null);
            StartCoroutine(AcquireTarget(recipient));
        }

        private IEnumerator AcquireTarget(BulletHoming recipient) {
            var waitedSeconds = 0f;
            yield return new WaitWhile(() => {
                var target = _enemyManager.enemies
                    .OrderBy(enemy => (enemy.transform.position - recipient.transform.position).sqrMagnitude)
                    .FirstOrDefault(enemy => !_targetList.ContainsValue(enemy) && !enemy.isEnemyDying);

                if (target == null) {
                    waitedSeconds += Time.deltaTime;
                    if (!(waitedSeconds >= 3)) return true;
                    
                    StartCoroutine(recipient.SelfDestruct());
                    return false;
                }

                _targetList[recipient] = target;   
                recipient.SetDestination(target.transform.position);
                return false;
            });
        }

        private void HandleOnTargetDie(EnemyBase enemy) {
            if (!_targetList.ContainsValue(enemy)) return;
            
            var registeredRecipient 
                = _targetList.FirstOrDefault(list => list.Value == enemy).Key;
            
            registeredRecipient.StopTracking();
            StartCoroutine(AcquireTarget(registeredRecipient));
        }

        private void HandleOnTargetHit(BulletHoming recipient) {
            _targetList.Remove(recipient);
        }
    }
}
