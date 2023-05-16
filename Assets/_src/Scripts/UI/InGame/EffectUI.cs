using System;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.UI.InGame.ItemEffects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

namespace _src.Scripts.UI.InGame {
    public class EffectUI : MonoBehaviour {
        [Header("Refs")]
        public CanvasGroup mainUI;
        public GraphicRaycaster shopRaycaster;

        [Header("Effects")] 
        public AirStrikeUI airStrikeUI;

        private void Start() {
            EventDispatcher
                .instance.SubscribeListener(EventType.OnEffectItem, itemTag => OpenEffect((ShopItemTag) itemTag));
            EventDispatcher
                .instance.SubscribeListener(EventType.ReOpenUI, _ => ReOpenUI());
            
            airStrikeUI.PreInitAirStrikeUI();
        }
        
        private void OpenEffect(ShopItemTag itemTag) {
            shopRaycaster.enabled = false;
            mainUI
                .DOFade(0, 0.8f)
                .OnComplete(() => {
                    switch (itemTag) {
                        case ShopItemTag.AirStrike:
                            airStrikeUI.gameObject.SetActive(true);
                            airStrikeUI.InitAirStrikeUI();
                            break;
                        case ShopItemTag.Nuke:
                            break;
                        case ShopItemTag.HitoriGotoh:
                            break;
                    }
                });
        }

        private void ReOpenUI() {
            mainUI.DOFade(1, 0.8f).OnComplete(() => {
                airStrikeUI.gameObject.SetActive(false);
                shopRaycaster.enabled = false;
                EventDispatcher.instance.SendMessage(EventType.OnEffectFinish);
            });
        }
    }
}
