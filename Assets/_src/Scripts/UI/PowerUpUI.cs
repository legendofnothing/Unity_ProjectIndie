using System;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _src.Scripts.UI {
    public class PowerUpUI : MonoBehaviour {
        public List<Button> powerUpButtons = new();
        
        private void Start() {
            this.SubscribeListener(EventType.SwitchToPlayer, _=>EnableButton());
            this.SubscribeListener(EventType.SwitchToShooting, _=>DisableButton());
        }
        
        //Disable button on enemy turn
        private void EnableButton() {
            foreach (var button in powerUpButtons) {
                button.interactable = true;
                button.GetComponent<Image>().DOFade(255, 0.8f);
            }
        }
        
        //Enable button on enemy turn
        private void DisableButton() {
            foreach (var button in powerUpButtons) {
                button.interactable = false;
                button.GetComponent<Image>().DOFade(0, 0.8f);
            }
        }
        
        public void Health() {
            this.SendMessage(EventType.PowerupHealth);
        }

        public void DamageBuff() {
            this.SendMessage(EventType.PowerupDamageBuff);
        }

        public void Explosion() {
            this.SendMessage(EventType.PowerupExplosion);
        }

        public void Nuke() {
            this.SendMessage(EventType.PowerupExplosion);
        }
    }
}
