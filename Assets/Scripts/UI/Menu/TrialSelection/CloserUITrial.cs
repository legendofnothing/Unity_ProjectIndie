using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Menu.TrialSelection {
    public class CloserUITrial : MonoBehaviour {
        public Canvas closerCanvas;
        public CanvasGroup closerGroup;
        public Image closerBackground;
        public List<Canvas> closerTrials = new();
        public TextMeshProUGUI text;

        private void Start() {
            closerCanvas.enabled = false;
            closerBackground.color = Color.clear;
            closerGroup.alpha = 0;

            foreach (var trial in closerTrials) {
                trial.enabled = false;
            }
        }

        public void Close(int id) {
            var key = id switch {
                0 => SceneName.Outskirts,
                1 => SceneName.AbandonedValley,
                2 => SceneName.GatesOfHell,
                _ => SceneName.ThisIsIt
            };

            const float muffleDuration = 1.2f;
            AudioManager.instance.MuffleMusic(false, muffleDuration);
            DOVirtual.DelayedCall(muffleDuration + 1f, () => {
                AudioManager.instance.LowerMusic(AudioManager.MasterOption.Mute, muffleDuration * 2);
            }).OnComplete(() => {
                AudioManager.instance.StopMusic();
                AudioManager.instance.MuffleMusic(true);
                AudioManager.instance.LowerMusic(AudioManager.MasterOption.Unmute);
            });

            closerCanvas.enabled = true;
            closerBackground
                .DOColor(Color.black, 1f)
                .OnComplete(() => {
                    closerTrials[id].enabled = true;
                    closerGroup
                        .DOFade(1, 1.2f)
                        .OnComplete(() => {
                            DOVirtual.DelayedCall(1f, () => {
                                text
                                    .DOColor(Color.clear, 0.5f)
                                    .OnComplete(() => {
                                        SceneManager.LoadScene(key);
                                    });
                            });
                        });
                });
        }
    }
}