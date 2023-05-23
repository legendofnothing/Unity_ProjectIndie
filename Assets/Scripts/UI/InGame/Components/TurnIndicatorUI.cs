using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher = Scripts.Core.EventDispatcher.EventDispatcher;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame.Components {
    public class TurnIndicatorUI : MonoBehaviour {
        public enum TurnIndicator {
            Player,
            Enemy,
            Shop,
            Death
        }
        
        [Serializable]
        public struct TurnIndicatorData {
            public TurnIndicator indicator;
            public Sprite icon;
            public Color backgroundColor;
        }

        public GameObject background;
        public Image backgroundImage;
        public Image indicatorIcon;
        
        [Space]
        
        public List<TurnIndicatorData> data;
        private bool _hasDied;

        private void Start() {
            EventDispatcher.instance
                .SubscribeListener(EventType.SwitchToPlayer, _=>SwitchIndicator(TurnIndicator.Player));
            
            EventDispatcher.instance
                .SubscribeListener(EventType.SwitchToEnemy, _=>SwitchIndicator(TurnIndicator.Enemy));
            
            EventDispatcher.instance
                .SubscribeListener(EventType.OpenShop, _=>SwitchIndicator(TurnIndicator.Shop));
            
            EventDispatcher.instance
                .SubscribeListener(EventType.SwitchToEnd, _=> {
                    _hasDied = true;
                    SwitchIndicator(TurnIndicator.Death);
                });

            backgroundImage.color = data[0].backgroundColor;
            indicatorIcon.sprite = data[0].icon;
        }

        private void SwitchIndicator(TurnIndicator indicator) {
            if (_hasDied) return;
            var currData = data.Find(d => d.indicator == indicator);
            var originalPos = background.transform.localPosition;
            var s = DOTween.Sequence();
            s
                .Append(background.transform.DOLocalMove(originalPos - new Vector3(0, 10f, 0), 1.5f)
                    .SetEase(Ease.Linear).OnComplete(() => {
                        indicatorIcon.sprite = currData.icon;
                        indicatorIcon.DOFade(1, 0.2f);
                    }))
                .Insert(0, indicatorIcon.DOFade(0, 1.4f))
                .Append(background.transform.DOLocalMove(originalPos, 0.8f).SetEase(Ease.OutQuint))
                .Insert(0.6f, backgroundImage.DOColor(currData.backgroundColor, 0.4f));
        }
    }
}
