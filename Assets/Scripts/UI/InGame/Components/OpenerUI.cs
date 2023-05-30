using System;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame.Components {
    public class OpenerUI : MonoBehaviour {
        public Image backgroundImage;
        public Canvas openerCanvas;
        public CanvasGroup trialGroup;

        private void Start() {
            openerCanvas.enabled = true;
            trialGroup.alpha = 1f;
            backgroundImage.color = Color.black;
            trialGroup.transform
                .DOLocalMoveY(0, 1.2f)
                .OnComplete(() => {
                    trialGroup.DOFade(0, 1f)
                    .OnComplete(() => {
                        backgroundImage
                            .DOColor(Color.clear, 1.5f)
                            .OnComplete(() => {
                                openerCanvas.enabled = false;
                                LevelManager.instance.StartGame();
                            });
                    });
                });
        }
    }
}
