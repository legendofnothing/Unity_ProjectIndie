using System;
using DG.Tweening;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame {
    [Serializable]
    public struct Overlays {
        public Canvas canvas;
        public Image image;
    }

    public class OverlayHandler : Singleton<OverlayHandler> {
        [Header("Refs")] 
        public Overlays dimBackground;

        private void Start() {
            dimBackground.canvas.enabled = false;
            dimBackground.image.color -= new Color(0, 0, 0, 1);
        }

        public void OnDim(float alpha , float duration = 0.8f) {
            if (SaveSystem.UseFancyUI) {
                dimBackground.canvas.enabled = true;
                SetOverlayDim(dimBackground.image
                    , alpha
                    , duration
                    , () => {
                        if (alpha > 0) dimBackground.canvas.enabled = true;
                    }
                    , () => {
                        if (alpha <= 0) dimBackground.canvas.enabled = false;
                    });
            }

            else {
                dimBackground.canvas.enabled = alpha > 0;
                dimBackground.image.color = new Color(0, 0, 0, alpha); 
            }

        }

        #region Helper Function

        private void SetOverlayColor(Image image, Color color, float tweenDuration = 0f) {
            if (SaveSystem.UseFancyUI) {
                image.DOColor(color, tweenDuration);
            }

            else image.color = color;
        }
        
        private static void SetOverlayDim(Image image, float alpha, float tweenDuration = 0f, TweenCallback callbackStart = null, TweenCallback callbackEnd = null) {
            if (SaveSystem.UseFancyUI) {
                image
                    .DOFade(alpha, tweenDuration)
                    .OnStart(callbackStart)
                    .OnComplete(callbackEnd);
            }

            else {
                var c = image.color;
                image.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        #endregion
    }
}
