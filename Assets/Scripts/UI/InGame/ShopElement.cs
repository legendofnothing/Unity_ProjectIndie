using DG.Tweening;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame {
    public class ShopElement : MonoBehaviour {
        [Header("Refs")] 
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemCost;
        public TextMeshProUGUI itemDescriptor;
        public Image itemSprite;

        [Space] 
        public CanvasGroup priceTag;
        public CanvasGroup soldBg;

        private ShopItem _currShopItem;
        private Sequence currPriceTagSequence;
        private bool _hasBuy;
        
        public void Init(ShopItem item) {
            _currShopItem = item;
            
            priceTag.alpha = 1;
            soldBg.alpha = 0;

            itemName.text = item.itemName;
            itemCost.text = item.itemCost.ToString();
            itemDescriptor.text = item.itemDescriptor;
            itemSprite.sprite = item.itemSprite;
            _hasBuy = false;
        }

        public void OnBuy() {
            if (_hasBuy) return;
            _hasBuy = true;

            priceTag.DOFade(0, 1.2f);
            soldBg.DOFade(1, 1.2f);
            EventDispatcher.instance.SendMessage(EventType.OnItemBought, _currShopItem);
        }
    }
}
