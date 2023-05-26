using System;
using System.Collections.Generic;
using Bullet;
using DG.Tweening;
using Managers;
using ScriptableObjects;
using Scripts.Bullet;
using Scripts.Core;
using Scripts.Core.Collections;
using TMPro;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher = Scripts.Core.EventDispatcher.EventDispatcher;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame {
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
        [Header("Refs")]
        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public ShopItemData shopData;
        public List<ShopElement> shopElements;
        private readonly WeightedList<ShopItem> _weightedShopItems = new();

        private void Start() {
            canvas.enabled = false;
            canvasGroup.alpha = 0;
            
            EventDispatcher.instance.SubscribeListener(EventType.OpenShop, _=>OpenShop());
            EventDispatcher.instance.SubscribeListener(EventType.CloseShop, _=>OnProceed());
            EventDispatcher.instance.SubscribeListener(EventType.OnItemBought, item => HandleItem((ShopItem) item));

            foreach (var item in shopData.shopItems) {
                _weightedShopItems.AddElement(item.shopItem, item.chanceToAppear);
            }
        }

        private void OpenShop() {
            foreach (var element in shopElements) {
                element.Init(_weightedShopItems.GetRandomItem());
            }
            
            canvas.enabled = true;
            canvasGroup.DOFade(1, 1f);
        }

        public void OnProceed() {
            canvasGroup.DOFade(0, 1f).OnComplete(() => {
                canvas.enabled = false;
                EventDispatcher.instance.SendMessage(EventType.SwitchToPlayer);
            });
        }

        private void HandleItem(ShopItem item) {
            switch (item.itemTag) {
                case ShopItemTag.Bullet:
                    BulletManager.instance.AddBullet(item.prefab);
                    break;
                case ShopItemTag.Health:
                    if (Player.Player.instance.GetHealth + item.itemValue >= Player.Player.instance.hp) {
                        Player.Player.instance.SetHealth(Player.Player.instance.hp);
                    }
                    
                    else Player.Player.instance.AddHealth(item.itemValue);
                    break;
                case ShopItemTag.HealthSpecial:
                    if (Player.Player.instance.GetHealth + item.itemValue >= Player.Player.instance.hp) {
                        Player.Player.instance.SetHealth(Player.Player.instance.hp + 100f);
                    }
                    
                    else Player.Player.instance.AddHealth(item.itemValue);
                    break;
            }
        }
    }
}
