using DG.Tweening;
using Scripts.Core.EventDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.Components {
    public class BarUI : MonoBehaviour {
        public enum Type {
            Health,
        }

        public struct Message {
            public Type type;
            public float percentage;
            
            public Message(Type type, float percentage) {
                this.type = type;
                this.percentage = percentage;
            }
        }
        
        public Type type;
        public Slider bar;
        [Header("Tween Settings")] 
        public bool useTween = true;
        public Ease easeType = Ease.OutCirc;
        public float duration = 1.2f;

        private void Start() {
            EventDispatcher.instance
                .SubscribeListener(EventType.OnBarUIChange, msg => SetValue((Message) msg));
        }

        private void SetValue(Message msg) {
            if (type != msg.type) return;
            var value = msg.percentage;
            switch (useTween) {
                case true:
                    bar.DOValue(value, duration).SetEase(easeType);
                    break;
                default:
                    bar.value = value;
                    break;
            }
        }
    }
}
