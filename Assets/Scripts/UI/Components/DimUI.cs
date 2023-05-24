using System;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.Components {
    public class DimUI : MonoBehaviour {
        public struct Message {
            public bool isOpen;
            public float duration;
            
            public Message(bool isOpen, float duration) {
                this.isOpen = isOpen;
                this.duration = duration;
            }
        }

        public Canvas canvas;
        public Image image;
        public float dimValue;
        
        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.OnDimUI, msg => {
                var cast = (Message)msg;
                OnDim(cast.duration, cast.isOpen);
            });
        }

        private void OnDim(float duration, bool isOpen) {
            switch (isOpen) {
                case true:
                    canvas.enabled = true;
                    image.DOFade(dimValue, duration).SetUpdate(true);
                    break;
                
                default:
                    image.DOFade(0, duration).OnComplete(() => canvas.enabled = false).SetUpdate(true);
                    break;
            }
        }
    }
}
