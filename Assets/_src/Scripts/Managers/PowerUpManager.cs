
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy;
using UnityEngine;
using Random = System.Random;

namespace _src.Scripts.Managers {
    public class PowerUpManager : MonoBehaviour {
        public LevelData LevelData;
        
        private const float baseHp = 10;
        private const float baseDamageModifier = 1.2f;

        private EnemyManager _enemyManager;

        private void Awake() {
            _enemyManager = GetComponent<EnemyManager>();
        }

        private void Start() {
            this.SubscribeListener(EventType.PowerupHealth, _ => AddHealth());
            this.SubscribeListener(EventType.PowerupDamageBuff, _=> BuffDamage());
            this.SubscribeListener(EventType.PowerupExplosion, _=>Explosion());
        }

        private void AddHealth() {
            this.SendMessage(EventType.AddPlayerHealth, baseHp);
        }

        private void BuffDamage() {
            this.SendMessage(EventType.BuffBullet, baseDamageModifier);
        }
        
        //Not Working Yet
        private void Explosion() {
            var rnd = new Random();
            var randomEnemy = _enemyManager._enemies.ElementAt(rnd.Next(_enemyManager._enemies.Count()));
            
            UnityEngine.Debug.Log(randomEnemy.name);

            var circleOverlaps = Physics2D.OverlapCircleAll(
                randomEnemy.transform.position
                , 3f
                , LayerMask.NameToLayer("Enemy"));

            foreach (var hits in circleOverlaps) {  
                hits.gameObject.GetComponent<EnemyBase>().TakeDamage(25f);
            }
        }
    }
}
