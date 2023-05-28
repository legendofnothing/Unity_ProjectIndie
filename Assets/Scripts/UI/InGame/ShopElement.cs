using System;
using DG.Tweening;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using TMPro;
using UI.Components;
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
        
        private Color _originalBuyButtonColor;
        private Image _buyButtonImage;

        private void Start() {
            _buyButtonImage = priceTag.GetComponent<Image>();
            _originalBuyButtonColor = _buyButtonImage.color;
        }

        public void Init(ShopItem item) {
            _currShopItem = item;
            
            priceTag.alpha = 1;
            soldBg.alpha = 0;

            itemName.text = item.itemName;
            itemCost.text = UIStatic.ConvertCost(item.itemCost);
            itemDescriptor.text = item.itemDescriptor;
            itemSprite.sprite = item.itemSprite;
            _hasBuy = false;
        }

        public void OnBuy() {
            if (_hasBuy) return;
            
            if (SaveSystem.playerData.Coin - _currShopItem.itemCost <= 0) {
                currPriceTagSequence?.Kill();
                _buyButtonImage.color = _originalBuyButtonColor;
                currPriceTagSequence = DOTween.Sequence();
                currPriceTagSequence
                    .Append(_buyButtonImage.DOColor(Color.red, 0.15f))
                    .SetLoops(6, LoopType.Yoyo);
                
                return;
            }

            _hasBuy = true;

            SaveSystem.playerData.Coin -= _currShopItem.itemCost;
            SaveSystem.SaveData();
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
            
            priceTag.DOFade(0, 1.2f);
            soldBg.DOFade(1, 1.2f);
            EventDispatcher.instance.SendMessage(EventType.OnItemBought, _currShopItem);
        }
    }
}