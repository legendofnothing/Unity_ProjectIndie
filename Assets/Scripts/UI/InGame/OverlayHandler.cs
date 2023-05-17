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
        [HideInInspector] public bool isActive;
    }

    public class OverlayHandler : MonoBehaviour {
        [Header("Refs")] 
        public Overlays dimBackground;

        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.DimBackground, _ => OnDim());
            dimBackground.canvas.enabled = false;
            dimBackground.image.color -= new Color(0, 0, 0, 1);
        }

        private void OnDim() {
            if (SaveSystem.UseFancyUI) {
                if (!dimBackground.canvas.enabled) {
                    dimBackground.canvas.enabled = true;
                    SetOverlayDim(dimBackground.image
                        , dimBackground.isActive ? 0f : 0.6f
                        ,0.8f
                        , () => {
                            dimBackground.isActive = !dimBackground.isActive;
                        });
                }

                else {
                    SetOverlayDim(dimBackground.image
                        , dimBackground.isActive ? 0f : 0.6f
                        ,0.8f
                        , () => {
                            dimBackground.isActive = !dimBackground.isActive;
                            dimBackground.canvas.enabled = false;
                        });
                }
            }

            else {
                dimBackground.isActive = !dimBackground.isActive;
                dimBackground.canvas.enabled = !dimBackground.isActive;
                SetOverlayDim(dimBackground.image
                    , dimBackground.isActive ? 0f : 0.6f);
            }
        }

        #region Helper Function

        private void SetOverlayColor(Image image, Color color, float tweenDuration = 0f) {
            if (SaveSystem.UseFancyUI) {
                image.DOColor(color, tweenDuration);
            }

            else image.color = color;
        }
        
        private static void SetOverlayDim(Image image, float alpha, float tweenDuration = 0f, TweenCallback callback = null) {
            if (SaveSystem.UseFancyUI) {
                image
                    .DOFade(alpha, tweenDuration)
                    .OnComplete(callback);
            }

            else {
                var c = image.color;
                image.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        #endregion
    }
}
