using System;
using DG.Tweening;
using Scripts.Core;
using TMPro;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Components {
    public class UpgradePlayerUI : MonoBehaviour {
        public enum UpgradeType {
            HP,
            DEF,
            ATK,
            CRIT
        }
        
        [Header("Settings")] 
        public UpgradeType type;
        
        [Header("Refs")]
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public CanvasGroup notEnoughMoneyPrompt;
        public Slider levelUpEffect;
        
        private const int _baseCost = 1200;
        private int _currCost;

        private Sequence _notEnoughMoneySeq;
        private Sequence _levelUpSeq;
        private bool _canAdd = true;

        private void Start() {
            notEnoughMoneyPrompt.alpha = 0;
            levelText.SetText($"Lvl.{GetLevel()}"); 
            _currCost = GetLevel() == 1 ? _baseCost : (int) (_baseCost * GetLevel() * GetCostModifier());
            costText.SetText(ConvertCost(_currCost));
            levelUpEffect.value = 0;
        }

        public void OnUpgrade() {
            if (!_canAdd) return;
            if (SaveSystem.playerData.Coin - _currCost < 0) {
                _notEnoughMoneySeq?.Kill();
                _notEnoughMoneySeq = DOTween.Sequence();
                _notEnoughMoneySeq
                    .Append(notEnoughMoneyPrompt.DOFade(1, 0.1f).SetEase(Ease.InSine))
                    .Append(DOVirtual.DelayedCall(1.3f, null))
                    .Append(notEnoughMoneyPrompt.DOFade(0, 0.08f).SetEase(Ease.InSine));
            }

            else {
                _canAdd = false;
                SaveSystem.playerData.Coin = SaveSystem.playerData.Coin - _currCost;
                SaveData();

                _levelUpSeq = DOTween.Sequence();
                _levelUpSeq
                    .Append(levelUpEffect.DOValue(1, 0.6f).SetEase(Ease.InSine))
                    .Append(DOVirtual.DelayedCall(0.2f, () => { 
                        levelText.SetText($"Lvl.{GetLevel()}");
                        levelUpEffect.value = 0;
                        _canAdd = true;
                    }));

                _currCost = (int) (_baseCost * GetLevel() * GetCostModifier());
                costText.SetText(ConvertCost(_currCost));
                UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
            }
        }

        private string ConvertCost(float value) {
            if (value < 1000) return value.ToString("0");
            return value switch {
                >= 1000 and < 1000000 => (value / 1000).ToString("0.0") + "k",
                >= 1000000 and < 1000000000 => (value / 1000000).ToString("0.0") + "m",
                >= 1000000000 and < 1000000000000 => (value / 1000000000).ToString("0.0") + "b",
                >= 1000000000000 and < 1000000000000000 => (value / 1000000000).ToString("0.0") + "t",
                _ => "I'm guessing a lot"
            };
        }

        private void SaveData() {
            switch (type) {
                case UpgradeType.HP:
                    SaveSystem.playerData.PlayerLevels[PlayerStatLevels.HP] += 1;
                    break;
                
                case UpgradeType.ATK:
                    SaveSystem.playerData.PlayerLevels[PlayerStatLevels.ATK] += 1;
                    break;
                
                case UpgradeType.DEF:
                    SaveSystem.playerData.PlayerLevels[PlayerStatLevels.DEF] += 1;
                    break;
                
                default:
                    SaveSystem.playerData.PlayerLevels[PlayerStatLevels.CRIT] += 1;
                    break;
            }
            
            SaveSystem.SaveData();
        }

        private int GetLevel() {
            return type switch {
                UpgradeType.HP => SaveSystem.playerData.PlayerLevels[PlayerStatLevels.HP],
                UpgradeType.ATK => SaveSystem.playerData.PlayerLevels[PlayerStatLevels.ATK],
                UpgradeType.DEF => SaveSystem.playerData.PlayerLevels[PlayerStatLevels.DEF],
                _ => SaveSystem.playerData.PlayerLevels[PlayerStatLevels.CRIT],
            };
        }

        private float GetCostModifier() {
            return type switch {
                UpgradeType.HP => 1f,
                UpgradeType.ATK => 1.07f,
                UpgradeType.DEF => 1.14f,
                _ => 0.96f,
            };
        }
    }
}
