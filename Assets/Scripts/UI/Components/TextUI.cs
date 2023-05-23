using System;
using Scripts.Core.EventDispatcher;
using TMPro;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.Components {
    public class TextUI : MonoBehaviour {
        public enum Type {
            Health,
            Coin,
            AmmoCount,
            Turn,
            Score,
        }

        public struct Message {
            public Type type;
            public float value;

            public Message(Type type, float value) {
                this.type = type;
                this.value = value;
            }
        }

        public Type type;
        public TextMeshProUGUI textComp;

        private void Start() {
            EventDispatcher.instance
                .SubscribeListener(EventType.OnTextUIChange, msg => SetText((Message) msg));
        }

        private void SetText(Message msg) {
            if (type != msg.type) return;
            var value = msg.value;
            
            switch (type) {
                case Type.Health:
                    textComp.SetText(value.ToString("0.0") + "/" + Player.Player.instance.hp);
                    break;
                case Type.AmmoCount:
                    textComp.SetText("x" + value);
                    break;
                case Type.Coin:
                    if (value < 1000) textComp.SetText(value.ToString("0"));
                    else {
                        switch (value) {
                            case >= 1000 and < 1000000:
                                textComp.SetText((value/1000).ToString("0.0") + "k");
                                break;
                            case >= 1000000 and < 1000000000:
                                textComp.SetText((value/1000000).ToString("0.0") + "m");
                                break;
                            case >= 1000000000 and < 1000000000000:
                                textComp.SetText((value/1000000000).ToString("0.0") + "b");
                                break;
                            case >= 1000000000000 and < 1000000000000000:
                                textComp.SetText((value/1000000000).ToString("0.0") + "t");
                                break;
                            default:
                                textComp.SetText("I'm guessing a lot");
                                break;
                        }
                    }
                    break;
                default:
                    textComp.SetText(value.ToString("0"));
                    break;
            }
        }
    }
}
