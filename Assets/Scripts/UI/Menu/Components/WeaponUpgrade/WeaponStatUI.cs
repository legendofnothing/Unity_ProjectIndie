using System;
using DG.Tweening;
using Scripts.Core;
using TMPro;
using UI.Components;
using UnityEngine;

namespace UI.Menu.Components.WeaponUpgrade {
    public class WeaponStatUI : MonoBehaviour {
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI fireRateText;
        public TextMeshProUGUI ammoCountText;

        [Space] 
        public TextMeshProUGUI atkCostText;
        public TextMeshProUGUI frCostText;
        public TextMeshProUGUI amCostText;
        
        [Space]
        public TextMeshProUGUI alertId1;
        public TextMeshProUGUI alertId2;
        public TextMeshProUGUI alertId3;

        private Sequence[] _sequences = {null, null, null};

        private WeaponHandlerUI manager;
        
        private void Start() {
            manager = WeaponHandlerUI.instance;
            for (var i = 0; i < 3; i++) {
                UpdateCost(i);
            }
            UpdateText();
        }

        public void OnBuy(int id) {
            var cost = WeaponHandlerUI.baseCost * WeaponHandlerUI.GetModifier(id) * manager.GetLevel(id);
            if (SaveSystem.playerData.Coin - Mathf.RoundToInt(cost) <= 0) {
                SetAlert(id);
                return;
            }

            SaveSystem.playerData.Coin -= Mathf.RoundToInt(cost);
            SaveSystem.SaveData();
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
            
            switch (id) {
                case 0:
                    manager.gunStats.damageLevel += 1;
                    break;
                
                case 1:
                    manager.gunStats.fireRateLevel += 1;
                    break;
                
                default:
                    manager.gunStats.ammoCountLevel += 1;
                    break;
            }
            
            UpdateCost(id);
            UpdateText();
            manager.UpdateStat();
        }

        private void UpdateText() {
            attackText
                .SetText(manager.gunStats.damageLevel == 1 
                    ? "0%" 
                    : $"{Math.Round(WeaponHandlerUI.damageModifier * manager.gunStats.damageLevel, 1)}%");
            
            fireRateText
                .SetText(manager.gunStats.fireRateLevel == 1 
                    ? "0%" 
                    : $"{Math.Round(WeaponHandlerUI.fireRateModifier * manager.gunStats.fireRateLevel, 1)}%");
            
            ammoCountText
                .SetText(manager.gunStats.ammoCountLevel == 1 
                    ? "0%" 
                    : $"{Math.Round(WeaponHandlerUI.ammoCountModifier * manager.gunStats.ammoCountLevel, 1)}%");
        }

        private void UpdateCost(int id) {
            switch (id) {
                case 0:
                    atkCostText.SetText(UIStatic
                        .ConvertCost(WeaponHandlerUI.baseCost * WeaponHandlerUI.GetModifier(id) * manager.GetLevel(id)));
                    break;
                
                case 1:
                    frCostText.SetText(UIStatic
                        .ConvertCost(WeaponHandlerUI.baseCost * WeaponHandlerUI.GetModifier(id) * manager.GetLevel(id)));
                    break;
                
                default:
                    amCostText.SetText(UIStatic
                        .ConvertCost(WeaponHandlerUI.baseCost * WeaponHandlerUI.GetModifier(id) * manager.GetLevel(id)));
                    break;
            }
        }

        private void SetAlert(int id) {
            var text = id switch {
                0 => alertId1,
                1 => alertId2,
                _ => alertId3
            };
            
            _sequences[id]?.Kill();
            _sequences[id] = DOTween.Sequence();
            _sequences[id]
                .Append(text.DOFade(1, 0.3f))
                .Append(DOVirtual.DelayedCall(1f, null))
                .Append(text.DOFade(0, 0.1f));
        }
    }
}
