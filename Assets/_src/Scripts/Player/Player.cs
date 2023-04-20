using System;
using _src.Scripts.Bullet;
using UnityEngine;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using Unity.Collections;
using UnityEngine.Serialization;

namespace _src.Scripts.Player {
    public class Player : Singleton<Player> {
        public float hp;
        private float _currentHp;
        
        [Space]
        public PlayerController input;
        public BulletManager bulletManager;

        [Space] public float offsetToCamera; 
        
        [HideInInspector] public Camera camera;
        
        private void Start() {
            camera = Camera.main;
            
            var unitsPerPixel = offsetToCamera / Screen.width;
            var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            camera.orthographicSize = desiredHalfHeight;
            
            _currentHp = hp;
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
