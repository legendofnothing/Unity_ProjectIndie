using System;
using System.Collections.Generic;
using Bullet;
using DG.Tweening;
using Newtonsoft.Json;
using ScriptableObjects;
using Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//GOD FORBIDS ANYONE TO READ THE UI SCRIPTS

namespace UI.Menu.Components.WeaponUpgrade {
    public class WeaponHandlerUI : Singleton<WeaponHandlerUI> {
        public GunData gunData;

        [Header("Gun Display")] 
        public Image gunImage;
        public TextMeshProUGUI gunName;
        public TextMeshProUGUI gunDescription;
        [Space] 
        public TextMeshProUGUI statTextAtk;
        public TextMeshProUGUI statTextFr;
        public TextMeshProUGUI statTextAmmo;

        private Sequence _currSequence;
        [HideInInspector] public bool isRunning;
        [HideInInspector] public int equippedWeapon;
        [HideInInspector] public GunStats gunStats;
        
        public const float damageModifier = 0.5f;
        public const float fireRateModifier = 0.4f;
        public const float ammoCountModifier = 0.2f;

        public const int baseCost = 30000;
        private void Awake() {
            if (!PlayerPrefs.HasKey(DataKey.PlayerEquippedWeapon)) {
                equippedWeapon = 0;
                PlayerPrefs.SetInt(DataKey.PlayerEquippedWeapon, equippedWeapon);
            }

            else {
                equippedWeapon = PlayerPrefs.GetInt(DataKey.PlayerEquippedWeapon);
            }
            
            if (!PlayerPrefs.HasKey(DataKey.PlayerWeapon)) {
                gunStats = new GunStats() {
                    damageLevel = 1,
                    fireRateLevel = 1,
                    ammoCountLevel = 1,
                    isAimingGuideUnlocked = false,
                    isAmmoPouchUnlocked = false,
                    isExtraDamageUnlocked = false
                };
                PlayerPrefs.SetString(DataKey.PlayerWeapon, JsonConvert.SerializeObject(gunStats));
            }

            else {
                gunStats = JsonConvert.DeserializeObject<GunStats>(PlayerPrefs.GetString(DataKey.PlayerWeapon));
            }
        }

        private void Start() {
            UpdateStat();
        }

        public void SwitchWeapon(int index) {
            if (isRunning) return;
            isRunning = true;
            var currData = gunData.gunInfos[index];
            var prevData = gunData.gunInfos[equippedWeapon];
            
            equippedWeapon = index;
            PlayerPrefs.SetInt(DataKey.PlayerEquippedWeapon, equippedWeapon);
            
            float targetDamage;
            float targetAmmoCount;

            if (gunStats.isExtraDamageUnlocked) {
                targetDamage = gunStats.damageLevel == 1
                    ? currData.baseAttack + currData.baseAttack * 0.1f
                    : currData.baseAttack + currData.baseAttack * 0.1f +
                      (float)Math.Round(damageModifier * gunStats.damageLevel, 1);
            }
            else {
                targetDamage = gunStats.damageLevel == 1
                    ? currData.baseAttack 
                    : currData.baseAttack + (float)Math.Round(damageModifier * gunStats.damageLevel, 1);
            }
            
            if (gunStats.isAmmoPouchUnlocked) {
                targetAmmoCount = gunStats.ammoCountLevel == 1
                    ? currData.baseAmmoCount + currData.baseAmmoCount * 0.25f
                    : currData.baseAmmoCount + currData.baseAmmoCount * 0.25f +
                      (float)Math.Round(ammoCountModifier * gunStats.ammoCountLevel, 1);
            }
            else {
                targetAmmoCount = gunStats.ammoCountLevel == 1
                    ? currData.baseAmmoCount 
                    : currData.baseAmmoCount + (float)Math.Round(ammoCountModifier * gunStats.ammoCountLevel, 1);
            }
            
            var targetFireRate =
                gunStats.fireRateLevel == 1
                    ? currData.baseFireRate
                    : currData.baseFireRate + (float)Math.Round(fireRateModifier * gunStats.fireRateLevel, 1);
            
            _currSequence?.Kill();
            _currSequence = DOTween.Sequence();
            _currSequence
                .Append(gunImage.transform.DOLocalMoveY(-20, 0.5f))
                .Append(DOVirtual.DelayedCall(0.1f, () => {
                    gunName.SetText(currData.gunSprite.name == "AssaultRifle" ? "Assault Rifle" : currData.gunSprite.name);
                    gunImage.sprite = currData.gunSprite;
                    gunDescription.SetText(currData.gunDescription);
                    gunImage.SetNativeSize();
                    gunImage.transform.localScale = currData.gunSprite.name == "Rifle"
                        ? new Vector3(1.5f, 1.5f, 1)
                        : new Vector3(2, 2, 1);
                }))
                .Append(gunImage.transform.DOLocalMoveY(0, 0.5f))
                .Insert(0.5f, DOVirtual.Int((int)prevData.baseAttack, Mathf.FloorToInt(targetDamage), 0.3f, value => {
                    statTextAtk.SetText(value + "\nDMG");
                }))
                .Insert(0.5f, DOVirtual.Int((int)prevData.baseFireRate, Mathf.FloorToInt(targetFireRate), 0.3f, value => {
                    statTextFr.SetText(value + "\nRPM/s");
                }))
                .Insert(0.5f, DOVirtual.Int((int)prevData.baseAmmoCount, Mathf.FloorToInt(targetAmmoCount), 0.3f, value => {
                    statTextAmmo.SetText(value + "\nBLLs");
                }))
                .Insert(0.15f, gunImage.DOFade(0, 0.5f))
                .Insert(0.15f, gunDescription.DOFade(0, 0.5f))
                .Insert(0.15f, gunName.DOFade(0, 0.5f))
                .Insert(0.8f, gunImage.DOFade(1, 0.5f))
                .Insert(0.8f, gunDescription.DOFade(1, 0.5f))
                .Insert(0.8f, gunName.DOFade(1, 0.5f))
                .OnComplete(() => isRunning = false);
        }

        public void UpdateStat() {
            PlayerPrefs.SetString(DataKey.PlayerWeapon, JsonConvert.SerializeObject(gunStats));
            var currData = gunData.gunInfos[equippedWeapon];

            float targetDamage;
            float targetAmmoCount;

            if (gunStats.isExtraDamageUnlocked) {
                targetDamage = gunStats.damageLevel == 1
                    ? currData.baseAttack + currData.baseAttack * 0.1f
                    : currData.baseAttack + currData.baseAttack * 0.1f +
                      (float)Math.Round(damageModifier * gunStats.damageLevel, 1);
            }
            else {
                targetDamage = gunStats.damageLevel == 1
                    ? currData.baseAttack 
                    : currData.baseAttack + (float)Math.Round(damageModifier * gunStats.damageLevel, 1);
            }
            
            if (gunStats.isAmmoPouchUnlocked) {
                targetAmmoCount = gunStats.ammoCountLevel == 1
                    ? currData.baseAmmoCount + currData.baseAmmoCount * 0.25f
                    : currData.baseAmmoCount + currData.baseAmmoCount * 0.25f +
                      (float)Math.Round(ammoCountModifier * gunStats.ammoCountLevel, 1);
            }
            else {
                targetAmmoCount = gunStats.ammoCountLevel == 1
                    ? currData.baseAmmoCount 
                    : currData.baseAmmoCount + (float)Math.Round(ammoCountModifier * gunStats.ammoCountLevel, 1);
            }
            
            var targetFireRate =
                gunStats.fireRateLevel == 1
                    ? currData.baseFireRate
                    : currData.baseFireRate + (float)Math.Round(fireRateModifier * gunStats.fireRateLevel, 1);
            

            statTextAtk.SetText(Mathf.FloorToInt(targetDamage) + "\nDMG");
            statTextFr.SetText(Mathf.FloorToInt(targetFireRate) + "\nRPM/s");
            statTextAmmo.SetText(Mathf.FloorToInt(targetAmmoCount) + "\nBLLs");
            
            

            statTextAtk.DOColor(gunStats.damageLevel == 1 
                ? gunStats.isExtraDamageUnlocked 
                    ? Color.yellow 
                    : Color.white
                : Color.yellow, 0.8f);
            statTextFr.DOColor(gunStats.fireRateLevel == 1 ? Color.white : Color.yellow, 0.8f);
            statTextAmmo.DOColor(gunStats.damageLevel == 1 
                ? gunStats.isAmmoPouchUnlocked 
                    ? Color.yellow 
                    : Color.white
                : Color.yellow, 0.8f);
        }

        public static float GetModifier(int id) {
            return id switch {
                0 => damageModifier,
                1 => fireRateModifier,
                _ => ammoCountModifier
            };
        }

        public int GetLevel(int id) {
            return id switch {
                0 => gunStats.damageLevel,
                1 => gunStats.fireRateLevel,
                _ => gunStats.ammoCountLevel
            };
        }
    }
}
