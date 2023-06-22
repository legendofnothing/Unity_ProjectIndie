using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Newtonsoft.Json;
using Scripts.Bullet;
using Scripts.Bullet.Types;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UI.Components;
using UnityEngine;
using AudioType = Managers.AudioType;
using EventType = Scripts.Core.EventDispatcher.EventType;
using Random = UnityEngine.Random;

namespace Bullet {
    [Serializable]
    public struct GunInfo {
        public Sprite gunSprite;
        public float offsetToPlayer;
        [Space] 
        public float baseAttack;
        public float baseFireRate;
        public int baseAmmoCount;
        [Space] 
        public string gunDescription;
        public int gunCost;
    }
    
    [Serializable]
    public class GunStats {
        public int damageLevel;
        public int fireRateLevel;
        public int ammoCountLevel;
        public bool isAimingGuideUnlocked;
        public bool isAmmoPouchUnlocked;
        public bool isExtraDamageUnlocked;
    }
    
    public class BulletManager : Singleton<BulletManager>
    {
        public enum Weapon {
            Pistol,
            Shotgun,
            Rifle,
            AutomaticRifle,
        }
        
        public List<GameObject> bulletList;

        private float _critChance; 
        private float _damageModifier;
        
        private float _damage = 100f;
        private int _bulletCap = 100;
        private float _fireRate = 100f;
        
        public const float damageModifier = 0.5f;
        public const float fireRateModifier = 0.4f;
        public const float ammoCountModifier = 0.2f;
        
        //Holds current bullet in the scene
        private List<GameObject> _currentList;
        //Store any new bullets being added
        private List<GameObject> _addedTempList;

        private Weapon _currWeapon;

        private void Awake(){
            _currentList = new List<GameObject>();
            _addedTempList = new List<GameObject>();
        }

        private void Start() {
            UIStatic.FireUIEvent(TextUI.Type.AmmoCount, bulletList.Count);
            EventDispatcher.instance.SubscribeListener(EventType.BulletDestroyed, bullet => OnBulletDestroyed((GameObject) bullet));
            
            //0 - pistol
            //1 - shotgun
            //2 - rife
            //3 - automatic rifle
            var currWeapon = PlayerPrefs.HasKey(DataKey.PlayerEquippedWeapon)
                ? PlayerPrefs.GetInt(DataKey.PlayerEquippedWeapon)
                : 0;

            _currWeapon = PlayerPrefs.GetInt(DataKey.PlayerEquippedWeapon) switch {
                1 => Weapon.Shotgun,
                2 => Weapon.Rifle,
                3 => Weapon.AutomaticRifle,
                _ => Weapon.Pistol
            };
            
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

            var currData = Player.Player.instance.gunData.gunInfos[currWeapon];
            if (gunStats.isExtraDamageUnlocked) {
                _damage = gunStats.damageLevel == 1
                    ? currData.baseAttack + currData.baseAttack * 0.1f
                    : currData.baseAttack + currData.baseAttack * 0.1f +
                      (float)Math.Round(damageModifier * gunStats.damageLevel, 1);
            }
            else {
                _damage = gunStats.damageLevel == 1
                    ? currData.baseAttack 
                    : currData.baseAttack + (float)Math.Round(damageModifier * gunStats.damageLevel, 1);
            }
            
            if (gunStats.isAmmoPouchUnlocked) {
                _bulletCap = gunStats.ammoCountLevel == 1
                    ? Mathf.CeilToInt(currData.baseAmmoCount + currData.baseAmmoCount * 0.25f)
                    : Mathf.CeilToInt(currData.baseAmmoCount + currData.baseAmmoCount * 0.25f +
                                      (float)Math.Round(ammoCountModifier * gunStats.ammoCountLevel, 1));
            }
            else {
                _bulletCap = gunStats.ammoCountLevel == 1
                    ? Mathf.CeilToInt(currData.baseAmmoCount)
                    : Mathf.CeilToInt(currData.baseAmmoCount 
                                      + (float)Math.Round(ammoCountModifier * gunStats.ammoCountLevel, 1));
            }
            
            _fireRate =
                gunStats.fireRateLevel == 1
                    ? currData.baseFireRate
                    : currData.baseFireRate + (float)Math.Round(fireRateModifier * gunStats.fireRateLevel, 1);
        }

        private void OnBulletDestroyed(GameObject bullet) {
            _currentList.Remove(bullet);
            if (_currentList.Count > 0) return;

            bulletList.AddRange(_addedTempList);
            UIStatic.FireUIEvent(TextUI.Type.AmmoCount, bulletList.Count);
            _addedTempList.Clear();

            //Switch to Enemy Turn
            EventDispatcher.instance.SendMessage(EventType.SwitchToEnemy);
        }
        
        
        //Spawn Bullet, called in PlayerController
        public IEnumerator SpawnBullet(Vector3 position, Quaternion rotation)
        {
            //Instantiate each bullet in the bulletList
            foreach (var bulletInst in bulletList.Select(bullet => Instantiate(bullet, position, rotation))) {
                //Add all instantiated bullet into the currentList
                _currentList.Add(bulletInst);

                //Set Bullet Damage w/ any modifiers
                var bulletComp = bulletInst.GetComponent<BulletBase>();
                bulletComp.damage = Random.Range(0f, 1f) < _critChance
                    ? _damage * bulletComp.damageModifier * 2 * _damageModifier
                    : _damage * bulletComp.damageModifier * _damageModifier;

                if (bulletComp.specialTag == BulletSpecialTag.Homing) 
                    TargetingSystem.instance.AddHomingBullet((BulletHoming) bulletComp);
                
                AudioManagerHelper.instance.PlayEffect(AudioType.BULLET_Shoot);

                yield return new WaitForSeconds(1 / (_fireRate/60f));
            }
            
            yield return null;
        }
        
        public void AddBullet(GameObject bullet) {
            if (_currentList.Count <= 0) {
                if (bulletList.Count >= _bulletCap) return;
                bulletList.Add(bullet);
                UIStatic.FireUIEvent(TextUI.Type.AmmoCount, bulletList.Count);
            }
            else _addedTempList.Add(bullet);
        }
        
        public void AddBulletOnScene(GameObject bullet) { 
            _currentList.Add(bullet);
        }

        public void ChangeDamageModifier(float amount) {
            _damageModifier = amount;
        }
        
        public void ChangeCritModifier(float percent) {
            _critChance = percent;
        }
    }
}
