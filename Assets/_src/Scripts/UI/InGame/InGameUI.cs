using System;
using System.Globalization;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.UI.InGame;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _src.Scripts.UI {
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

        [Header("Screen Effect")] 
        public Image lowHealthEffect;
        public float lowHealthThreshold;
        public float lowHealthFadeAlpha;
        public float lowHealthDuration;
        public Ease lowHealthEaseType;
        private Sequence _currLowHealthTweenSequence;
        
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
            
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerHpChange, hp => OnLowHealthEffect((float) hp));
            
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerDie, _=>OnPlayerDie());

            healthBarRect.color = originalHealthBarColor;
            var hpColor = lowHealthEffect.color;
            lowHealthEffect.color = new Color(hpColor.r, hpColor.g, hpColor.b, 0);
        }

        private void InitUI(UIInitData data) {
            _originalPlayerHp = data.PlayerHp;
            _currentPlayerHp = data.PlayerHp;

            _previousCoins = data.PlayerCoins;
            _previousScore = 0;

            healthBar.value = 0;
            scoreDisplay.text = "0";
            healthBar.DOValue(1, startupDuration);
            DOVirtual.Float(0, _originalPlayerHp, startupDuration, value => { 
                healthDisplay.text = value.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0");
            });
            DOVirtual.Float(0, _previousCoins, startupDuration, value => {
                coinDisplay.text = value.ToString("0");
            });
        }

        private void OnPlayerHpChange(float currentHp) {

            if (currentHp > _originalPlayerHp) {
                if (_playerHealthState != PlayerHealthStateUI.Overcharged) {
                    _playerHealthState = PlayerHealthStateUI.Overcharged;
                    healthBarRect.DOColor(overchargedHealthBarColor, 0.4f);
                    healthBar.DOValue(1, startupDuration);
                }
            }

            else {
                if (_playerHealthState != PlayerHealthStateUI.Normal) {
                    _playerHealthState = PlayerHealthStateUI.Normal;
                    healthBarRect.DOColor(originalHealthBarColor, 0.4f);
                }
            }

            switch (_playerHealthState) {
                case PlayerHealthStateUI.Normal:
                    healthBar.DOValue(currentHp / _originalPlayerHp, startupDuration).SetUpdate(true);
                    DOVirtual.Float(_currentPlayerHp, currentHp, startupDuration, value => {
                        if (_currentPlayerHp <= 0) {
                            _currentPlayerHp = 0;
                            healthDisplay.text = "0.0/" + _originalPlayerHp.ToString("0.0");
                        }
                        else {
                            _currentPlayerHp = value;
                            healthDisplay.text = value.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0");
                        }
                    }).SetUpdate(true);
                    break;
                
                case PlayerHealthStateUI.Overcharged:
                    DOVirtual.Float(_currentPlayerHp, currentHp, startupDuration, value => {
                        if (_currentPlayerHp <= 0) {
                            _currentPlayerHp = 0;
                            healthDisplay.text = "0.0/" + _originalPlayerHp.ToString("0.0");
                        }
                        else {
                            _currentPlayerHp = value;
                            healthDisplay.text = value.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0");
                        }
                    }).SetUpdate(true);
                    break;
            }
        }

        private void OnCoinChange(int currentCoins) {
            DOVirtual.Float(_previousCoins, currentCoins, 1.4f, value => {
                _previousCoins = value;
                coinDisplay.text = value.ToString("0");
            });
        }
        
        private void OnScoreChange(int currentScore) {
            DOVirtual.Float(_previousScore, currentScore, 1.4f, value => {
                _previousScore = value;
                scoreDisplay.text = value.ToString("0");
            });
        }

        private void OnTurnNumberChange(int turnNumber) {
            turnDisplay.text = turnNumber.ToString("0");
        }

        private void OnLowHealthEffect(float currentHp) {
            var currentPercentage = currentHp / _originalPlayerHp;
            
            if (!(currentPercentage <= lowHealthThreshold)) return;
            if (_currLowHealthTweenSequence != null) return;
            
            _currLowHealthTweenSequence = DOTween.Sequence();
            _currLowHealthTweenSequence
                .Append(FadeLowHealthEffect())
                .PrependInterval(0.5f)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnPlayerDie() {
            SaveSystem.SaveData(SceneManager.GetActiveScene().name);
            _currLowHealthTweenSequence.Kill();
            lowHealthEffect.DOFade(1f, 0.8f).SetUpdate(true);
            endGameUI.TransitToDeathScene();
        }

        #region Helper Functions

        private Tween FadeLowHealthEffect() {
            return lowHealthEffect.DOFade(lowHealthFadeAlpha, lowHealthDuration).SetEase(lowHealthEaseType);
        }

        #endregion
    }
}
