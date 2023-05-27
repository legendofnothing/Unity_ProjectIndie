using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Core;
using TMPro;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Components.WeaponUpgrade {
    public class WeaponGearUI : MonoBehaviour {
        public enum GearType {
            AmmoPouch,
            AimingGuide,
            ImprovedCore,
        }
        
        [Serializable]
        public struct Gear {
            public GearType type;
            [Space]
            public GameObject selection;
            public Sprite icon;
            [Space]
            public string gearName;
            public string gearDescription;
            public int gearPrice;
        }

        public List<Gear> gears = new();
        [Header("Refs")] 
        public GameObject handler;
        public CanvasGroup textGroup;
        [Space]
        public Image icon;
        public TextMeshProUGUI gearName;
        public TextMeshProUGUI gearDescription;
        public TextMeshProUGUI gearPrice;
        [Space]
        public TextMeshProUGUI notEnoughAlert;
        [Space] 
        public Image buyButtonImage;
        public TextMeshProUGUI buyButtonText;

        [Header("Colors")] 
        public Color selectionUnlocked;
        public Color selectionLocked;
        [Space] 
        public Color buyButtonUnlocked;
        public Color buyButtonLocked;

        private WeaponHandlerUI manager;
        private int _currId;
        private Sequence _seq;
        private Sequence _alertSeq;

        private void Start() {
            manager = WeaponHandlerUI.instance;
            
            foreach (var gear in gears) {
                var image = gear.selection.GetComponent<Image>();
                image.color = gear.type switch {
                    GearType.AmmoPouch => manager.gunStats.isAmmoPouchUnlocked ? selectionUnlocked : selectionLocked,
                    GearType.AimingGuide => manager.gunStats.isAimingGuideUnlocked
                        ? selectionUnlocked
                        : selectionLocked,
                    GearType.ImprovedCore => manager.gunStats.isExtraDamageUnlocked
                        ? selectionUnlocked
                        : selectionLocked,
                    _ => image.color
                };
            }
            
            SelectGear(0);
            _currId = 0;
        }

        public void SelectGear(int id) {
            var prevID = _currId;
            _currId = id;

            var gear = GetGearByID(id);
            var prevGear = GetGearByID(prevID);
            
            var unlocked = gear.type switch {
                GearType.AmmoPouch => manager.gunStats.isAmmoPouchUnlocked,
                GearType.AimingGuide => manager.gunStats.isAimingGuideUnlocked,
                _=> manager.gunStats.isExtraDamageUnlocked
            };
            
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq
                .Append(buyButtonImage.DOColor(unlocked ? buyButtonUnlocked : buyButtonLocked, 0.5f))
                .Insert(0,  unlocked 
                    ? gearPrice.DOFade(0, 0.01f)
                    : gearPrice.DOFade(1, 0.01f))
                .Insert(0, DOVirtual.Float(prevGear.gearPrice, gear.gearPrice, 0.5f, value => {
                    gearPrice.SetText(UIStatic.ConvertCost(value));
                }))
                .Insert(0, handler.transform.DOLocalMoveY(gear.selection.transform.localPosition.y, 0.5f))
                .Insert(0, textGroup.DOFade(0, 0.1f).OnComplete(() => {
                    icon.sprite = gear.icon;
                    gearName.SetText(gear.gearName);
                    gearDescription.SetText(gear.gearDescription);
                }))
                .Insert(0, buyButtonText.DOFade(0, 0.1f).OnComplete(() => {
                    buyButtonText.SetText(unlocked ? "UNLOCKED" : "LOCKED");
                }))
                .Insert(0.6f, textGroup.DOFade(1, 0.1f))
                .Insert(0.6f, buyButtonText.DOFade(1, 0.1f));
        }

        public void UnlockGear() {
            var gear = GetGearByID(_currId);
            
            var unlocked = gear.type switch {
                GearType.AmmoPouch => manager.gunStats.isAmmoPouchUnlocked,
                GearType.AimingGuide => manager.gunStats.isAimingGuideUnlocked,
                _=> manager.gunStats.isExtraDamageUnlocked
            };
            
            if (unlocked) return;

            if (SaveSystem.playerData.Coin - gear.gearPrice <= 0) {
                _alertSeq?.Kill();
                _alertSeq = DOTween.Sequence();
                _alertSeq
                    .Append(notEnoughAlert.DOFade(1, 0.3f))
                    .Append(DOVirtual.DelayedCall(1f, null))
                    .Append(notEnoughAlert.DOFade(0, 0.1f));
                return;
            }

            SaveSystem.playerData.Coin -= gear.gearPrice;
            SaveSystem.SaveData();
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
            
            switch (gear.type) {
                case GearType.AmmoPouch:
                    manager.gunStats.isAmmoPouchUnlocked = true;
                    break;
                case GearType.AimingGuide:
                    manager.gunStats.isAimingGuideUnlocked = true;
                    break;
                case GearType.ImprovedCore:
                    manager.gunStats.isExtraDamageUnlocked = true;
                    break;
            }
            
            manager.UpdateStat();
            
            var image = gear.selection.GetComponent<Image>();
            var s = DOTween.Sequence();
            s
                .Append(image.DOColor(selectionUnlocked, 0.2f))
                .Insert(0, buyButtonImage.DOColor(buyButtonUnlocked, 0.2f))
                .Insert(0, buyButtonText.DOFade(0, 0.1f)
                    .OnComplete(() => buyButtonText.SetText("UNLOCKED")
                    ))
                .Insert(0.6f, buyButtonText.DOFade(1, 0.1f))
                .Insert(0, gearPrice.DOFade(0, 0.1f));
        }

        private Gear GetGearByID(int id) {
            return id switch {
                0 => gears.Find(g => g.type == GearType.AmmoPouch),
                1 => gears.Find(g => g.type == GearType.AimingGuide),
                _ => gears.Find(g => g.type == GearType.ImprovedCore)
            };
            
        } 
    }
}
