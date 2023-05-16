using DG.Tweening;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.UI.InGame {
    public class ShopElement : MonoBehaviour {
        [Header("Refs")] 
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemCost;
        public TextMeshProUGUI itemDescriptor;
        public Image itemSprite;

        [Space] 
        public CanvasGroup priceTag;
        public CanvasGroup soldTag;

        private ShopItem _currShopItem;
        private Image priceTagImage;
        private Sequence currPriceTagSequence;

        private void Start() {
            priceTagImage = priceTag.GetComponent<Image>();
        }

        public void Init(ShopItem item) {
            _currShopItem = item;
            priceTag.alpha = 1;
            soldTag.alpha = 0;

            itemName.text = item.itemName;
            itemCost.text = item.itemCost.ToString();
            itemDescriptor.text = item.itemDescriptor;
            itemSprite.sprite = item.itemSprite;
        }

        public void OnBuy() {
            if (SaveSystem.playerData.Coin - _currShopItem.itemCost < 0) {
                if (currPriceTagSequence != null) return;
                
                currPriceTagSequence = DOTween.Sequence();
                currPriceTagSequence
                    .Append(priceTagImage.DOColor(new Color(255, 0, 0), 0.1f))
                    .SetLoops(4, LoopType.Yoyo)
                    .PrependInterval(0.1f)
                    .OnComplete(() => currPriceTagSequence = null);
            }

            else {
                priceTag.DOFade(0, 0.2f);
                soldTag.DOFade(1, 0.8f);
                Player.Player.instance.ReduceCoin(_currShopItem.itemCost);
                EventDispatcher.instance.SendMessage(EventType.OnItemBought, _currShopItem);
            }
        }
    }
}
