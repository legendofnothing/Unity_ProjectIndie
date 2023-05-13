using System;
using System.Collections.Generic;
using _src.Scripts.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.UI.DeathScreenUI { 
    public class DeathScreenUI : MonoBehaviour {
        public TextMeshProUGUI previousSceneName;
        public List<TextMeshProUGUI> statTexts;
        private int _currentSetIndex;

        private PlayerData _playerData;

        private void Awake() {
            Time.timeScale = 1;
            SaveSystem.Init();
            _playerData = SaveSystem.playerData;
        }

        private void Start() {
            previousSceneName.text = _playerData.PreviousSceneName;

            foreach (var text in statTexts) {
                text.text = "0";
            }
        }

        public void StartDisplayingStats() {
            var currText = statTexts[_currentSetIndex];
            currText.text = "0";
            
            var valueToSwitch = 0;
            var duration = 3.3f;
            switch (_currentSetIndex) {
                case 0:
                    valueToSwitch = _playerData.LevelData[_playerData.PreviousSceneName].Score;
                    break;
                case 1:
                    duration = 1.2f;
                    valueToSwitch = _playerData.LevelData[_playerData.PreviousSceneName].TurnNumber;
                    break;
                case 2:
                    valueToSwitch = _playerData.Coin;
                    break;
            }

            DOVirtual.Float(0f, valueToSwitch, duration, value => {
                currText.text = value.ToString("0");
            }).OnComplete(() => {
                _currentSetIndex++;
                if (_currentSetIndex >= statTexts.Count) return;
                StartDisplayingStats();
            });
        }
    }
}
