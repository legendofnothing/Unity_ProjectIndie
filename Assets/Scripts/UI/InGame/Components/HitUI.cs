using System;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame.Components {
    public class HitUI : MonoBehaviour {
        public Image image;
        private Sequence _sequence;
        
        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.OnPlayerHPChange,_ => StartHurt());
            gameObject.SetActive(false);
        }

        private void StartHurt() {
            image.DOFade(0, 0);
            gameObject.SetActive(true);
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _sequence
                .SetUpdate(true)
                .Append(image.DOFade(0.1f, 0.5f).SetEase(Ease.OutQuint))
                .Append(image
                    .DOFade(0f, 1.2f)
                    .OnComplete(() => gameObject.SetActive(false))
                    .SetEase(Ease.InOutQuint));
        }
    }
}
