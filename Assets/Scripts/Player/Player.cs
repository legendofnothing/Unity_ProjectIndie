using DG.Tweening;
using Scripts.Bullet;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using Scripts.UI.InGame;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.Player {
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
        [Space] public float offsetToCamera;
        [HideInInspector] public Camera camera;
        [HideInInspector] public FloatScreenPosition screenFloats;

        private Tween _currentCameraShakeTween;

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

            screenFloats = new FloatScreenPosition(
                camera.ViewportToWorldPoint(Vector3.one).y
                ,camera.ViewportToWorldPoint(Vector3.one).x
                ,camera.ViewportToWorldPoint(Vector3.zero).x
                ,camera.ViewportToWorldPoint(Vector3.zero).y);
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
            
            EventDispatcher.instance.SendMessage(EventType.OnInitUI, new UIInitData() { 
                PlayerHp = _currentHp,
                PlayerCoins = SaveSystem.playerData.Coin
            });
        }

        public void TakeDamage(float amount) {
            _currentHp -= amount * _defendModifier;
            EventDispatcher.instance.SendMessage(EventType.OnPlayerHpChange, _currentHp);
            
            DoCameraShake(0.5f, 1.2f);

            if (!(_currentHp <= 0)) return;
            _currentHp = 0;
            EventDispatcher.instance.SendMessage(EventType.SwitchToEnd);
        }
        
        public void AddHealth(float amount) {
            _currentHp += amount;
            EventDispatcher.instance.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }
        
        public void SetHealth(float amount) {
            _currentHp = amount;
            EventDispatcher.instance.SendMessage(EventType.OnPlayerHpChange, _currentHp);
        }

        public float GetHealth => _currentHp;

        public void AddCoin(int amount) {
            SaveSystem.playerData.Coin += amount;
            EventDispatcher.instance.SendMessage(EventType.OnPlayerCoinChange, SaveSystem.playerData.Coin);
        }

        public void ReduceCoin(int amount) {
            SaveSystem.playerData.Coin -= amount;
            EventDispatcher.instance.SendMessage(EventType.OnPlayerCoinChange, SaveSystem.playerData.Coin);
        }
        
        public void AddScore(int amount) {
            SaveSystem.currentLevelData.Score += amount * SaveSystem.currentLevelData.TurnNumber;
            EventDispatcher.instance.SendMessage(EventType.OnScoreChange, SaveSystem.currentLevelData.Score);
        }

        public void DoCameraShake(float strength, float duration = 1f) {
            _currentCameraShakeTween = camera.DOShakePosition(duration, strength).OnComplete(() => {
                camera.transform.DOMove(new Vector3(0, 0, camera.transform.position.z), 0.8f)
                    .OnComplete(() => _currentCameraShakeTween = null);
            });
        }
    }
}
