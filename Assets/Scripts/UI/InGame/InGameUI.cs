using DG.Tweening;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame {
    public class UIInitData {
        public float PlayerHp;
        public float PlayerCoins;
    }

    public enum PlayerHealthStateUI {
        Normal,
        Overcharged
    }
    
    public class InGameUI : MonoBehaviour {
        [Header("Refs")] 
        public EndGameUI endGameUI;
        
        [Header("Configs")] 
        public float startupDuration;

        public Color originalHealthBarColor;
        public Color overchargedHealthBarColor;
        
        [Header("In-game Elements")] 
        public Slider healthBar;
        public Image healthBarRect;
        [Space]
        public TextMeshProUGUI healthDisplay;
        public TextMeshProUGUI scoreDisplay;
        public TextMeshProUGUI coinDisplay;
        public TextMeshProUGUI turnDisplay;
        
        
        private float _originalPlayerHp;
        private float _currentPlayerHp;

        private float _previousScore;
        private float _previousCoins;

        private PlayerHealthStateUI _playerHealthState;

        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.OnInitUI, hp => InitUI((UIInitData) hp));
            
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerHpChange, hp => OnPlayerHpChange((float) hp));
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerCoinChange, coin => OnCoinChange((int) coin));
            EventDispatcher.instance.SubscribeListener(EventType.OnScoreChange, score => OnScoreChange((int) score));
            EventDispatcher.instance.SubscribeListener(EventType.OnTurnNumberChange, turn => OnTurnNumberChange((int) turn));
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerDie, _=>OnPlayerDie());
            
            //EventDispatcher.instance.SubscribeListener(EventType.OnPlayerHpChange, hp => OnLowHealthEffect((float) hp));
            

            healthBarRect.color = originalHealthBarColor;
            // var hpColor = lowHealthEffect.color;
            // lowHealthEffect.color = new Color(hpColor.r, hpColor.g, hpColor.b, 0);
        }

        private void InitUI(UIInitData data) {
            _originalPlayerHp = data.PlayerHp;
            _currentPlayerHp = data.PlayerHp;

            _previousCoins = data.PlayerCoins;
            _previousScore = 0;

            healthBar.value = 0;
            scoreDisplay.text = "0";

            if (SaveSystem.UseFancyUI) {
                healthBar.DOValue(1, startupDuration);
                DOVirtual.Float(0, _originalPlayerHp, startupDuration, value => { 
                    healthDisplay.SetText(value.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0"));
                });
                DOVirtual.Float(0, _previousCoins, startupDuration, value => {
                    coinDisplay.SetText(value.ToString("0"));
                });
            }
            else {
                healthBar.value = 1;
                healthDisplay.SetText(_originalPlayerHp.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0"));
                coinDisplay.SetText(_previousCoins.ToString("0"));
            }
        }

        private void OnPlayerHpChange(float currentHp) {

            if (currentHp > _originalPlayerHp) {
                if (_playerHealthState != PlayerHealthStateUI.Overcharged) {
                    _playerHealthState = PlayerHealthStateUI.Overcharged;

                    switch (SaveSystem.UseFancyUI) {
                        case true:
                            healthBarRect.DOColor(overchargedHealthBarColor, 0.4f);
                            healthBar.DOValue(1, startupDuration);
                            break;
                        default:
                            healthBarRect.color = overchargedHealthBarColor;
                            healthBar.value = 1;
                            break;
                    }
                }
            }

            else {
                if (_playerHealthState != PlayerHealthStateUI.Normal) {
                    _playerHealthState = PlayerHealthStateUI.Normal;
                    healthBarRect.DOColor(originalHealthBarColor, 0.4f);
                    
                    switch (SaveSystem.UseFancyUI) {
                        case true:
                            healthBarRect.DOColor(originalHealthBarColor, 0.4f);
                            break;
                        default:
                            healthBarRect.color = originalHealthBarColor;
                            break;
                    }
                }
            }

            switch (_playerHealthState) {
                case PlayerHealthStateUI.Normal:
                    
                    switch (SaveSystem.UseFancyUI) {
                        case true:
                            healthBar.DOValue(currentHp / _originalPlayerHp, startupDuration).SetUpdate(true);
                            DOVirtual.Float(_currentPlayerHp, currentHp, startupDuration, value => {
                                if (_currentPlayerHp <= 0) {
                                    _currentPlayerHp = 0;
                                    healthDisplay.SetText("0.0/" + _originalPlayerHp.ToString("0.0"));
                                }
                                else {
                                    _currentPlayerHp = value;
                                    healthDisplay.SetText(value.ToString("0.0") + "/" +
                                                          _originalPlayerHp.ToString("0.0"));
                                }
                            }).SetUpdate(true);
                            break;
                        
                        default:
                            healthBar.value = currentHp / _originalPlayerHp;
                            if (_currentPlayerHp <= 0) {
                                _currentPlayerHp = 0;
                                healthDisplay.SetText("0.0/" + _originalPlayerHp.ToString("0.0"));
                            }
                            else {
                                _currentPlayerHp = currentHp;
                                healthDisplay.SetText(currentHp.ToString("0.0") + "/" +
                                                      _originalPlayerHp.ToString("0.0"));
                            }
                            break;
                    }
                    
                    break;
                
                case PlayerHealthStateUI.Overcharged:
                    switch (SaveSystem.UseFancyUI) {
                        case true:
                            DOVirtual.Float(_currentPlayerHp, currentHp, startupDuration, value => {
                                _currentPlayerHp = value;
                                healthDisplay.SetText(value.ToString("0.0") + "/" +
                                                      _originalPlayerHp.ToString("0.0"));
                            }).SetUpdate(true);
                            break;
                        
                        default:
                            _currentPlayerHp = currentHp;
                            healthDisplay.text = currentHp.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0");
                            break;
                    }
                    break;
            }
        }

        private void OnCoinChange(int currentCoins) {
            if (SaveSystem.UseFancyUI) {
                DOVirtual.Float(_previousCoins, currentCoins, 1.4f, value => {
                    _previousCoins = value;
                    coinDisplay.SetText(value.ToString("0"));
                });
            }

            else {
                _previousCoins = currentCoins;
                coinDisplay.SetText(currentCoins.ToString("0"));
            }
        }
        
        private void OnScoreChange(int currentScore) {
            if (SaveSystem.UseFancyUI) {
                DOVirtual.Float(_previousScore, currentScore, 1.4f, value => {
                    _previousScore = value;
                    scoreDisplay.SetText(value.ToString("0"));
                });
            }

            else {
                _previousScore = currentScore;
                scoreDisplay.SetText(currentScore.ToString("0"));
            }
        }

        private void OnTurnNumberChange(int turnNumber) {
            turnDisplay.SetText(turnNumber.ToString("0"));
        }

        private void OnPlayerDie() {
            SaveSystem.SaveData(SceneManager.GetActiveScene().name);
            endGameUI.TransitToDeathScene();
        } 
    }
}
