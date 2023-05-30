using System;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame.Components {
    public class CloserUI : MonoBehaviour {
        public GameUI gameUI;
        [Space]
        public Canvas canvas;
        public Image closerImage;

        [Header("Prompts")] 
        public CanvasGroup returnGroup;
        public CanvasGroup exitGroup;

        public enum CloserType {
            DeathScene,
            ReturnToMenu,
            ExitGame,
        }

        private void Start() {
            canvas.enabled = false;
            closerImage.fillAmount = 0;
            
            returnGroup.GetComponent<Canvas>().enabled = false;
            returnGroup.alpha = 0;
            
            exitGroup.GetComponent<Canvas>().enabled = false;
            exitGroup.alpha = 0;
            
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToEnd, _ => {
                Close(CloserType.DeathScene);
            });
        }

        public void Close(CloserType type) {
            canvas.enabled = true;
            closerImage.fillAmount = 0;
            var s = DOTween.Sequence();
            s
                .Append(DOVirtual.Float(gameUI.currTimeScale, 0, 1.6f, value => {
                    Time.timeScale = value;
                }))
                .SetUpdate(true)
                .Insert(0.3f, DOVirtual.Float(0f, 1f, 2f, value => {
                    closerImage.fillAmount = value;
                }))
                .SetUpdate(true)
                .OnComplete(() => {
                    DOTween.KillAll();
                    switch (type) {
                        case CloserType.ReturnToMenu:
                            returnGroup.GetComponent<Canvas>().enabled = true;
                            returnGroup.DOFade(1, 0.1f).SetUpdate(true);
                            DOVirtual.DelayedCall(3f, () => {
                                returnGroup
                                    .DOFade(0, 0.1f)
                                    .SetUpdate(true)
                                    .OnComplete(() => {
                                        Time.timeScale = 1;
                                        SceneManager.LoadScene("Menu");
                                    });
                            }).SetUpdate(true);
                            break;
                        
                        case CloserType.ExitGame:
                            exitGroup.GetComponent<Canvas>().enabled = true;
                            exitGroup.DOFade(1, 0.1f).SetUpdate(true);
                            DOVirtual.DelayedCall(1.5f, () => {
                                exitGroup
                                    .DOFade(0, 0.1f)
                                    .SetUpdate(true)
                                    .OnComplete(Application.Quit);
                            }).SetUpdate(true);
                            break;
                        
                        default:
                            SceneManager.LoadScene("DeathScene");
                            break;
                    }
                });
        }

        public void CloseWithoutTimeScale(CloserType type) {
            closerImage.fillAmount = 0;
            canvas.enabled = true;
            var s = DOTween.Sequence();
            s
                .Append(DOVirtual.Float(0f, 1f, 1.2f, value => {
                    closerImage.fillAmount = value;
                }))
                .OnComplete(() => {
                    DOTween.KillAll();
                    switch (type) {
                        case CloserType.ReturnToMenu:
                            returnGroup.GetComponent<Canvas>().enabled = true;
                            returnGroup.DOFade(1, 0.1f);
                            DOVirtual.DelayedCall(3f, () => {
                                returnGroup
                                    .DOFade(0, 0.1f)
                                    .OnComplete(() => {
                                        Time.timeScale = 1;
                                        SceneManager.LoadScene("Menu");
                                    });
                            });
                            break;
                        
                        case CloserType.ExitGame:
                            exitGroup.GetComponent<Canvas>().enabled = true;
                            exitGroup.DOFade(1, 0.1f);
                            DOVirtual.DelayedCall(1.5f, () => {
                                exitGroup
                                    .DOFade(0, 0.1f)
                                    .OnComplete(Application.Quit);
                            });
                            break;
                        
                        default:
                            SceneManager.LoadScene("DeathScene");
                            break;
                    }
                });
        }
    }
}
