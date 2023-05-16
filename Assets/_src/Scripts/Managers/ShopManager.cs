using System;
using System.Collections;
using _src.Scripts.Bullet;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.UI.InGame;
using UnityEngine;

namespace _src.Scripts.Managers {
    public class ShopManager : MonoBehaviour {
        public int maxBuyCount;
        
        private int _currentBuyCount;
        private Player.Player _player;
        private bool _hasEffectFinished;
        
        private void Start() {
            _player = Player.Player.instance;
            EventDispatcher.instance
                .SubscribeListener(EventType.OnItemBought, item => OnItemBought((ShopItem) item));
            
            EventDispatcher.instance
                .SubscribeListener(EventType.OpenShop, _ => OnShopOpen());
            
            EventDispatcher.instance
                .SubscribeListener(EventType.OnEffectFinish, _ => {
                    if (_currentBuyCount < maxBuyCount) return;
                    _hasEffectFinished = true;
                });
        }

        private void OnShopOpen() {
            _currentBuyCount = 0;
            _hasEffectFinished = false;
            EventDispatcher.instance.SendMessage(EventType.OnBuyCountChange
                , new FloatPair(_currentBuyCount, maxBuyCount));
        }

        private void OnItemBought(ShopItem item) {
            switch (item.itemTag) {
                case ShopItemTag.Bullet:
                    BulletManager.instance.AddBullet(item.prefab);
                    AddBuyCount();
                    break;
                case ShopItemTag.Health:
                    AddBuyCount();
                    if (_player.GetHealth + item.itemValue >= _player.hp) {
                        _player.SetHealth(_player.hp);
                    }
                    else _player.AddHealth(item.itemValue);
                    break;
                case ShopItemTag.HealthSpecial:
                    AddBuyCount();
                    if (_player.GetHealth + item.itemValue >= _player.hp) {
                        _player.SetHealth(_player.hp + 100f);
                    }
                    else _player.AddHealth(item.itemValue);
                    break;
                default:
                    EventDispatcher.instance.SendMessage(EventType.OnEffectItem, item.itemTag);
                    AddBuyCount(false);
                    break;
            }
        }

        private void AddBuyCount(bool exitImmediately = true) {
            _currentBuyCount++;
            if (_currentBuyCount >= maxBuyCount) {
                if (exitImmediately) {
                    EventDispatcher.instance.SendMessage(EventType.CloseShop);
                    StartCoroutine(DelayInput());
                }

                else StartCoroutine(AwaitEffectFinish());
            }

            else {
                EventDispatcher.instance.SendMessage(EventType.OnBuyCountChange
                    , new FloatPair(_currentBuyCount, maxBuyCount));
            }
        }

        private IEnumerator AwaitEffectFinish() {
            EventDispatcher.instance.SendMessage(EventType.CloseShop);
            yield return new WaitUntil(() => _hasEffectFinished);
            StartCoroutine(DelayInput());
        }
        
        private IEnumerator DelayInput() {
            Player.Player.instance.input.CanInput(false);
            yield return new WaitForSeconds(0.4f);
            EventDispatcher.instance.SendMessage(EnemyManager.instance.enemies.Count <= 0
                ? EventType.SwitchToEnemy
                : EventType.SwitchToPlayer);
        }
    }
}
