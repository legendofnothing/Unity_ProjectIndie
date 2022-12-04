using System;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Player {
    public class PlayerStats : MonoBehaviour {
        public float hp;
        private float _currentHp;

        private void Awake() {
            _currentHp = hp;
            
            if (hp == 0) UnityEngine.Debug.Log($"Set Player HP at {this}, now!");
        }

        private void Start() {
            this.SubscribeListener(EventType.EnemyDamagePlayer, param=>DealDamage((float) param));
            this.SubscribeListener(EventType.AddPlayerHealth, param => AddHealth((float) param));
            
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }

        private void DealDamage(float amount) {
            _currentHp -= amount;
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);

            if (_currentHp <= 0)
            {
                _currentHp = 0;
                this.SendMessage(EventType.SwitchToEnd);
            }
        }

        private void AddHealth(float amount) {
            _currentHp += amount;
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }
    }
}
