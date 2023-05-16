using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Core;
using Scripts.Core.Collections;
using Scripts.Managers;
using Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using EventDispatcher = Scripts.Core.EventDispatcher.EventDispatcher;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.UI.InGame {
    public enum ShopItemTag {
        Bullet,
        Health,
        HealthSpecial,
        AirStrike,
        Nuke,
        HitoriGotoh
    }
    
    [Serializable]
    public class ShopItem {
        [Header("Configs")] 
        public ShopItemTag itemTag;
        public int itemCost;

        [Space] 
        public string itemName;
        public string itemDescriptor;

        [Space] 
        public float itemValue;

        [Header("Refs")] 
        public Sprite itemSprite;
        public GameObject prefab;
    }
    
    public class ShopUI : MonoBehaviour {
        [Header("Misc")] 
        public GameObject aura;
        public TextMeshProUGUI buyCountText;
        
        [Header("Refs")]
        public GameObject container;
        public ShopItemData shopData;
        public List<ShopElement> shopElements;

        private Sequence _currAuraSequence;
        private readonly WeightedList<ShopItem> _weightedShopItems = new();

        private void Start() {
            container.SetActive(false);
            EventDispatcher.instance.SubscribeListener(EventType.OpenShop, _=>OpenShop());
            EventDispatcher.instance.SubscribeListener(EventType.CloseShop, _=>OnProceed());
            EventDispatcher.instance
                .SubscribeListener(EventType.OnBuyCountChange, obj => {
                    var data = (FloatPair)obj;
                    OnBuyCountChange(data.float1, data.float2);
                });

            foreach (var item in shopData.shopItems) {
                _weightedShopItems.AddElement(item.shopItem, item.chanceToAppear);
            }
        }

        private void OpenShop() {
            foreach (var element in shopElements) {
                element.Init(_weightedShopItems.GetRandomItem());
            }
            
            container.SetActive(true);
            _currAuraSequence = DOTween.Sequence();
            _currAuraSequence
                .Append(aura.transform.DOScale(new Vector3(1.1f, 1.2f), 1.4f))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnBuyCountChange(float currBuyCount, float maxBuyCount) {
            buyCountText.text = $"Purchases Left: {maxBuyCount - currBuyCount}/{maxBuyCount}";
        }

        public void OnProceed() {
            _currAuraSequence.Kill();
            container.SetActive(false);
            if (!ShopManager.instance.isAwaitingForFinish) StartCoroutine(ShopManager.instance.DelayInput());
        }
    }
}
