using System;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame.Components {
    public class TurnUIAnimation : MonoBehaviour {
        public GameObject hourGlass;
        [Header("Tween Settings")] 
        public Ease easeType = Ease.InOutQuint;
        public float duration = 1.4f;

        private void Start() {
            EventDispatcher.instance
                .SubscribeListener(EventType.SwitchToPlayer, _ => {
                    DOVirtual.Float(0, 360, duration, value => {
                        hourGlass.transform.eulerAngles = new Vector3(0, 0, value);
                    }).SetEase(easeType);
                });
        }
    }
}
