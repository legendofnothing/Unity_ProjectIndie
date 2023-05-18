using DG.Tweening;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UI.InGame.ItemEffects;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame {
    public class EffectUI : MonoBehaviour {
        [Header("Canvas")] 
        public Canvas airStrikeCanvas;
        public Canvas nukeCanvas;
        
        [Header("Refs")]
        public CanvasGroup mainUI;
        public GraphicRaycaster shopRaycaster;

        [Header("Effects")] 
        public AirStrikeUI airStrikeUI;
        public NuclearUI nuclearUI;

        private void Start() {
            EventDispatcher
                .instance.SubscribeListener(EventType.OnEffectItem, itemTag => OpenEffect((ShopItemTag) itemTag));
            EventDispatcher
                .instance.SubscribeListener(EventType.ReOpenUI, _ => ReOpenUI());
            
            airStrikeUI.PreInitAirStrikeUI();
            nuclearUI.PreInitNuclearUI();

            airStrikeCanvas.enabled = false;
            nukeCanvas.enabled = false;
        }
        
        private void OpenEffect(ShopItemTag itemTag) {
            shopRaycaster.enabled = false;
            mainUI.blocksRaycasts = false;
            OverlayHandler.instance.OnDim(0);

            if (SaveSystem.UseFancyUI) {
                mainUI
                    .DOFade(0, 0.8f)
                    .OnComplete(() => {
                        switch (itemTag) {
                            case ShopItemTag.AirStrike:
                                airStrikeCanvas.enabled = true;
                                airStrikeUI.InitAirStrikeUI();
                                break;
                            case ShopItemTag.Nuke:
                                nukeCanvas.enabled = true;
                                nuclearUI.InitNuclearUI();
                                break;
                            case ShopItemTag.HitoriGotoh:
                                break;
                        }
                    });
            }
            else {
                mainUI.alpha = 0;
                switch (itemTag) {
                    case ShopItemTag.AirStrike:
                        airStrikeCanvas.enabled = true;
                        airStrikeUI.InitAirStrikeUI();
                        break;
                    case ShopItemTag.Nuke:
                        nukeCanvas.enabled = true;
                        nuclearUI.InitNuclearUI();
                        break;
                    case ShopItemTag.HitoriGotoh:
                        break;
                }
            }
        }

        private void ReOpenUI() {
            if (SaveSystem.UseFancyUI) {
                mainUI.DOFade(1, 0.8f).OnComplete(() => {
                    airStrikeCanvas.enabled = false;
                    nukeCanvas.enabled = false;
                    shopRaycaster.enabled = true;
                    mainUI.blocksRaycasts = true;
                    OverlayHandler.instance.OnDim(0.6f);
                    EventDispatcher.instance.SendMessage(EventType.OnEffectFinish);
                });
            }

            else {
                DOVirtual.DelayedCall(0.8f, () => {
                    mainUI.alpha = 1;
                    airStrikeCanvas.enabled = false;
                    nukeCanvas.enabled = false;
                    shopRaycaster.enabled = true;
                    mainUI.blocksRaycasts = true;
                    OverlayHandler.instance.OnDim(0.6f);
                    EventDispatcher.instance.SendMessage(EventType.OnEffectFinish);
                });
            }
        }
    }
}
