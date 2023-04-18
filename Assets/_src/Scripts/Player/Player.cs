using System;
using _src.Scripts.Bullet;
using UnityEngine;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;

namespace _src.Scripts.Player {
    public class Player : Singleton<Player> {
        protected Camera Camera;
        
        public float hp;
        private float _currentHp;
        [Space]
        public PlayerController input;
        public BulletManager bulletManager;

        private void Awake() {
            _currentHp = hp;
            Camera = Camera.main;
        }
        
        public void TakeDamage(float amount) {
            _currentHp -= amount;
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);

            if (!(_currentHp <= 0)) return;
            _currentHp = 0;
            this.SendMessage(EventType.SwitchToEnd);
        }
        
        //Add Health to player
        public void AddHealth(float amount) {
            _currentHp += amount;
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }
    }
}
