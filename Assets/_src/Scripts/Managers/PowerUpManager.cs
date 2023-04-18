
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy;
using UnityEngine;
using Random = System.Random;

namespace _src.Scripts.Managers {
    public class PowerUpManager : MonoBehaviour {
        private const float BaseHp = 100;
        private const float BaseDamageModifier = 1.2f;

        private EnemyManager _enemyManager;

        private void Awake() {
            _enemyManager = GetComponent<EnemyManager>();
        }

        private void Start() {
            //Subscribe Events
            this.SubscribeListener(EventType.PowerupHealth, _ => AddHealth());
            this.SubscribeListener(EventType.PowerupDamageBuff, _=> BuffDamage());
        }

        private void AddHealth() {
            Player.Player.instance.AddHealth(BaseHp);
        }

        private void BuffDamage() {
            Player.Player.instance.bulletManager.ChangeDamageModifier(BaseDamageModifier);
        }
    }
}
