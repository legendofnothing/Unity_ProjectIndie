using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using _src.Scripts.Core.Collections;
using _src.Scripts.ScriptableObjects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using EventDispatcher = _src.Scripts.Core.EventDispatcher.EventDispatcher;

namespace _src.Scripts.UI.InGame {
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
        private int buyCount; 

        private void Start() {
            container.SetActive(false);
            EventDispatcher.instance.SubscribeListener(EventType.OpenShop, _=>OpenShop());
            EventDispatcher.instance.SubscribeListener(EventType.OnItemBought, _=>OnItemBuy());

            foreach (var item in shopData.shopItems) {
                _weightedShopItems.AddElement(item.shopItem, item.chanceToAppear);
            }
        }

        private void OpenShop() {
            foreach (var element in shopElements) {
                element.Init(_weightedShopItems.GetRandomItem());
            }
            
            container.SetActive(true);
            buyCountText.text = "Purchase Left: 3/3";
            _currAuraSequence = DOTween.Sequence();
            _currAuraSequence
                .Append(aura.transform.DOScale(new Vector3(1.1f, 1.2f), 1.4f))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnItemBuy() {
            buyCount++;
            if (buyCount >= 3) OnProceed();
            else {
                buyCountText.text = $"Purchases Left: {3 - buyCount}/3";
            }
        }

        public void OnProceed() {
            _currAuraSequence.Kill();
            container.SetActive(false);
            EventDispatcher.instance.SendMessage(EventType.SwitchToPlayer);
            StartCoroutine(DelayInput());
        }

        private IEnumerator DelayInput() {
            Player.Player.instance.input.CanInput(false);
            yield return new WaitForSeconds(0.4f);
            Player.Player.instance.input.CanInput(true);
        }
    }
}
