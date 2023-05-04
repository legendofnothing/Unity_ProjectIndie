using System;
using System.Globalization;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _src.Scripts.UI {
    public class UIInitData {
        public float PlayerHp;
        public float PlayerCoins;
    }
    
    public class InGameUI : MonoBehaviour {
        [Header("Configs")] 
        public float startupDuration;
        
        [Header("In-game Elements")] 
        public Slider healthBar;
        public TextMeshProUGUI healthDisplay;
        public TextMeshProUGUI scoreDisplay;
        public TextMeshProUGUI coinDisplay;
        public TextMeshProUGUI turnDisplay;

        private float _originalPlayerHp;
        private float _currentPlayerHp;

        private float _previousScore;
        private float _previousCoins;

        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.OnInitUI, hp => InitUI((UIInitData) hp));
            
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerHpChange, hp => OnPlayerHpChange((float) hp));
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerCoinChange, coin => OnCoinChange((int) coin));
            EventDispatcher.instance.SubscribeListener(EventType.OnScoreChange, score => OnScoreChange((int) score));
            EventDispatcher.instance.SubscribeListener(EventType.OnTurnNumberChange, turn => OnTurnNumberChange((int) turn));
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
            healthBar.DOValue(currentHp / _originalPlayerHp, 1.2f);
            DOVirtual.Float(_currentPlayerHp, currentHp, startupDuration, value => {
                _currentPlayerHp = value;
                healthDisplay.text = value.ToString("0.0") + "/" + _originalPlayerHp.ToString("0.0");
            });
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
    }
}
