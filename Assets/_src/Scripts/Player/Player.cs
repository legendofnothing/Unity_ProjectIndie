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
        private float _defendModifier;
        private float _attackModifier;
        private float _critChance;

        [Space]
        public PlayerController input;
        public BulletManager bulletManager;
        [Space] public float offsetToCamera;
        [HideInInspector] public Camera camera;

        private void Awake() {
            var screenHeightInInch =  Screen.height / Screen.dpi;
            camera = Camera.main;
            if (screenHeightInInch < 6) {
                var unitsPerPixel = offsetToCamera / Screen.width;
                var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
                camera.orthographicSize = desiredHalfHeight;
            }
            else {
                camera.orthographicSize = 5;
            }
        }

        private void Start() {
            SetupStats();
        }

        private void SetupStats() {
            var playerStats = SaveSystem.instance.playerData.PlayerLevels;

            _currentHp = hp + (1.5f * playerStats[PlayerStatLevels.HP]);
            _defendModifier = 1 / (1 + 0.015f * playerStats[PlayerStatLevels.DEF]);
            _attackModifier = 1 * (1 + 0.005f * playerStats[PlayerStatLevels.ATK]);
            _critChance = 0.005f * playerStats[PlayerStatLevels.CRIT];
            
            bulletManager.ChangeDamageModifier(_attackModifier);
            bulletManager.ChangeCritModifier(_critChance);
            
            EventDispatcher.instance.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }

        public void TakeDamage(float amount) {
            _currentHp -= amount * _defendModifier;
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);

            if (!(_currentHp <= 0)) return;
            _currentHp = 0;
            this.SendMessage(EventType.SwitchToEnd);
        }
        
        public void AddHealth(float amount) {
            _currentHp += amount;
            this.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }

        public void AddCoin(int amount) {
            SaveSystem.instance.playerData.Coin += amount;
            this.SendMessage(EventType.OnPlayerCoinChange, SaveSystem.instance.playerData.Coin);
        }

        public void ReduceCoin(int amount) {
            SaveSystem.instance.playerData.Coin -= amount;
            this.SendMessage(EventType.OnPlayerCoinChange, SaveSystem.instance.playerData.Coin);
        }
        
        public void AddScore(int amount) {
            SaveSystem.instance.currentLevelData.Score += amount 
                                                          * SaveSystem.instance.currentLevelData.TurnNumber;
            this.SendMessage(EventType.OnScoreChange, SaveSystem.instance.currentLevelData.Score);
        }
    }
}
