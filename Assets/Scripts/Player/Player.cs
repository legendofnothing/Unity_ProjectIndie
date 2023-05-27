using System.Collections.Generic;
using Bullet;
using DG.Tweening;
using Newtonsoft.Json;
using ScriptableObjects;
using Scripts.Bullet;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UI.Components;
using UI.InGame;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Player { public class Player : Singleton<Player> {
        public float hp;
        
        private float _currentHp;
        private float _defendModifier;
        private float _attackModifier;
        private float _critChance;

        [Space] 
        public GunData gunData;
        public PlayerController input;
        public BulletManager bulletManager;
        [Space]
        public Camera playerCamera;

        [Space] 
        public List<Sprite> playerSkins;
        public SpriteRenderer playerSprite;
        public GameObject aimingGuide;
        public GameObject gunAnchor;
        public SpriteRenderer gunSprite;

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

            var currSkin = PlayerPrefs.HasKey(DataKey.PlayerSkin)
                ? PlayerPrefs.GetInt(DataKey.PlayerSkin)
                : 0;
            playerSprite.sprite = playerSkins[currSkin];

            var currWeapon = PlayerPrefs.HasKey(DataKey.PlayerEquippedWeapon)
                ? PlayerPrefs.GetInt(DataKey.PlayerEquippedWeapon)
                : 0;

            gunSprite.sprite = gunData.gunInfos[currWeapon].gunSprite;
            gunAnchor.transform.localPosition
                = new Vector3(0, gunData.gunInfos[currWeapon].offsetToPlayer);
            
            var gunStats = PlayerPrefs.HasKey(DataKey.PlayerWeapon)
                ? JsonConvert.DeserializeObject<GunStats>(PlayerPrefs.GetString(DataKey.PlayerWeapon))
                : new GunStats {
                    damageLevel = 1,
                    fireRateLevel = 1,
                    ammoCountLevel = 1,
                    isAimingGuideUnlocked = false,
                    isAmmoPouchUnlocked = false,
                    isExtraDamageUnlocked = false
                };
            aimingGuide.SetActive(gunStats.isAimingGuideUnlocked);
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
