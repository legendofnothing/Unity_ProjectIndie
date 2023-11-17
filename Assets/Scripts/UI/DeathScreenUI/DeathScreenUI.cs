using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Scripts.Core;
using TMPro;
using UI.InGame.Components;
using UnityEngine;

namespace UI.DeathScreenUI { 
    public class DeathScreenUI : MonoBehaviour {
        public GameObject audioManager;
        public List<AudioClip> myReactionAudios;
        public AudioClip goodTryClip;
        [Space]
        public TextMeshProUGUI previousSceneName;
        public List<TextMeshProUGUI> statTexts;
        public CloserUI closer;
        private int _currentSetIndex;

        private PlayerData _playerData;

        private void Awake() {
            Time.timeScale = 1;
            SaveSystem.Init();
            _playerData = SaveSystem.playerData;
        }

        private void Start() {
            if (FindObjectOfType<AudioManager>() == null) {
                Instantiate(audioManager);
            }
            
            previousSceneName.text = _playerData.PreviousSceneName;

            var s = DOTween.Sequence();
            s
                .Append(DOVirtual.DelayedCall(0, () => {
                    var clip = myReactionAudios[Random.Range(0, myReactionAudios.Count)];
                    AudioManager.instance.PlayEffect(clip);
                }))
                .PrependInterval(0.5f)
                .AppendInterval(0.5f)
                .Append(DOVirtual.DelayedCall(0, () => {
                    var clip = myReactionAudios[Random.Range(0, myReactionAudios.Count)];
                    AudioManager.instance.PlayEffect(goodTryClip);
                }));

            foreach (var text in statTexts) {
                text.text = "0";
            }
        }

        public void StartDisplayingStats() {
            var currText = statTexts[_currentSetIndex];
            currText.text = "0";
            
            var valueToSwitch = 0;
            var duration = 0.1f;
            switch (_currentSetIndex) {
                case 0:
                    valueToSwitch = _playerData.LevelData[_playerData.PreviousSceneName].Score;
                    break;
                case 1:
                    duration = 0.1f;
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

        public void Retry() {
            closer.Retry(_playerData.PreviousSceneName);
        }
        
        public void Return() {
            closer.CloseWithoutTimeScale(CloserUI.CloserType.ReturnToMenu);
        }

        public void Exit() {
            closer.CloseWithoutTimeScale(CloserUI.CloserType.ExitGame);
        }
    }
}
