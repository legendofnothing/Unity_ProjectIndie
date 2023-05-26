using Bullet;
using DG.Tweening;
using Scripts.Bullet;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UI.Components;
using UI.InGame;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Player {
    public class FloatScreenPosition {
        public readonly float TopScreen    = 0f;
        public readonly float RightScreen  = 0f;
        public readonly float LeftScreen   = 0f;
        public float BottomScreen = 0f;

        public FloatScreenPosition(float t, float r, float l, float b) {
            TopScreen = t;
            RightScreen = r;
            LeftScreen = l;
            BottomScreen = b;
        }
    }
    
    public class Player : Singleton<Player> {
        public float hp;
        
        private float _currentHp;
        private float _defendModifier;
        private float _attackModifier;
        private float _critChance;

        [Space]
        public PlayerController input;
        public BulletManager bulletManager;
        [FormerlySerializedAs("camera")] [Space]
        public Camera playerCamera;
        [HideInInspector] public FloatScreenPosition screenFloats;

        private bool _hasDied;

        /**
        * Desired specs
         * Camera Size: 6
         * Res: 1080x1920
        */
        private void Awake() {
            var windowAspect = Screen.width / (float)Screen.height;
            const float desiredAspect = 1080f / 1920f;
            var scaleHeight = windowAspect / desiredAspect;

            if (scaleHeight < 1.0f) {  
                playerCamera.orthographicSize /= scaleHeight;
            }
        }

        private void Start() {
            SetupStats();

            screenFloats = new FloatScreenPosition(
                playerCamera.ViewportToWorldPoint(Vector3.one).y
                ,playerCamera.ViewportToWorldPoint(Vector3.one).x
                ,playerCamera.ViewportToWorldPoint(Vector3.zero).x
                ,playerCamera.ViewportToWorldPoint(Vector3.zero).y);
        }

        private void SetupStats() {
            var playerStats = SaveSystem.playerData.PlayerLevels;
            hp += 1.5f * playerStats[PlayerStatLevels.HP];

            _currentHp = hp;
            _defendModifier = 1 / (1 + 0.015f * playerStats[PlayerStatLevels.DEF]);
            _attackModifier = 1 * (1 + 0.005f * playerStats[PlayerStatLevels.ATK]);
            _critChance = 0.005f * playerStats[PlayerStatLevels.CRIT];
            
            bulletManager.ChangeDamageModifier(_attackModifier);
            bulletManager.ChangeCritModifier(_critChance);
            
            UIStatic.FireUIEvent(TextUI.Type.Health, _currentHp);
            UIStatic.FireUIEvent(BarUI.Type.Health, _currentHp / hp, true);
            UIStatic.FireUIEvent(TextUI.Type.Turn, SaveSystem.currentLevelData.TurnNumber);
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
            UIStatic.FireUIEvent(TextUI.Type.Score, 0);
        }

        public void TakeDamage(float amount) {
            if (_hasDied) return;
            _currentHp -= amount * _defendModifier;

            if (_currentHp <= 0) {
                _hasDied = true;
                _currentHp = 0;
                EventDispatcher.instance.SendMessage(EventType.SwitchToEnd);
            }
            
            UIStatic.FireUIEvent(TextUI.Type.Health, _currentHp);
            UIStatic.FireUIEvent(BarUI.Type.Health, _currentHp / hp, true);

            EventDispatcher.instance.SendMessage(EventType.OnPlayerHPChange, _currentHp);
        }
        
        public void AddHealth(float amount) {
            _currentHp += amount;
            UIStatic.FireUIEvent(TextUI.Type.Health, _currentHp);
            UIStatic.FireUIEvent(BarUI.Type.Health, _currentHp / hp, true);
        }
        
        public void SetHealth(float amount) {
            _currentHp = amount;
            UIStatic.FireUIEvent(TextUI.Type.Health, _currentHp);
            UIStatic.FireUIEvent(BarUI.Type.Health, _currentHp / hp, true);
        }

        public float GetHealth => _currentHp;

        public void AddCoin(int amount) {
            SaveSystem.playerData.Coin += amount;
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
        }

        public void ReduceCoin(int amount) {
            SaveSystem.playerData.Coin -= amount;
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
        }
        
        public void AddScore(int amount) {
            SaveSystem.currentLevelData.Score += amount * SaveSystem.currentLevelData.TurnNumber;
            UIStatic.FireUIEvent(TextUI.Type.Score, SaveSystem.currentLevelData.Score);
        }
    }
}
