using System;
using _src.Scripts.Bullet;
using UnityEngine;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.UI;
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

        /**
        * Desired specs
         * Camera Size: 6
         * Res: 1080x1920
        */
        private void Awake() {
            camera = Camera.main;

            var windowAspect = Screen.width / (float)Screen.height;
            const float desiredAspect = 1080f / 1920f;
            var scaleHeight = windowAspect / desiredAspect;

            if (scaleHeight < 1.0f) {  
                camera.orthographicSize /= scaleHeight;
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
            
            EventDispatcher.instance.SendMessage(EventType.OnInitUI, new UIInitData() {
                PlayerHp = _currentHp,
                PlayerCoins = SaveSystem.instance.playerData.Coin
            });
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
