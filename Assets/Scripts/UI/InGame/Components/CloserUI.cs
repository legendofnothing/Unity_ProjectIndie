using System;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
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

        private void Start() {
            canvas.enabled = false;
            closerImage.fillAmount = 0;
            
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToEnd, _ => {
                canvas.enabled = true;
                var s = DOTween.Sequence();
                s
                    .Append(DOVirtual.Float(gameUI.currTimeScale, 0, 1.6f, value => {
                        Time.timeScale = value;
                    }))
                    .SetUpdate(true)
                    .Append(DOVirtual.Float(0f, 1f, 2f, value => {
                        closerImage.fillAmount = value;
                    }))
                    .SetUpdate(true)
                    .OnComplete(() => {
                        DOTween.KillAll();
                        SceneManager.LoadScene("DeathScene");
                    });
            });
        }
    }
}
