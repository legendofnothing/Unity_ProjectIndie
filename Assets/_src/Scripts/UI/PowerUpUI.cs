using System;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.ScriptableObjects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts.UI {
    public enum PowerUpType
    {
        Health,
        DamageBuff
    }
    
    public class PowerUpUI : MonoBehaviour
    {
        [Serializable]
        public struct PowerUpButtons
        {
            public PowerUpType type;
            public Button button;
            public TextMeshProUGUI costText;
            public int cost;
        }

        public PlayerData playerData;

        [Space]
        public List<PowerUpButtons> powerUpButtons = new();
        
        private void Start() {
            InitButtons();
            
            this.SubscribeListener(EventType.SwitchToPlayer, _=>EnableButton());
            this.SubscribeListener(EventType.SwitchToShooting, _=>DisableButton());
        }

        private void InitButtons()
        {
            foreach (var powerUp in powerUpButtons)
            {
                powerUp.costText.text = $"{powerUp.cost}";
            }
        }

        #region Button Methods

        //Disable button on enemy turn
        private void EnableButton() {
            foreach (var powerUp in powerUpButtons) {
                powerUp.button.interactable = true;
                powerUp.button.GetComponent<Image>().DOFade(255, 0.8f);
            }
        }
        
        private void EnableButton(PowerUpType type)
        {
            var buttonToDisable = powerUpButtons.Single(button => button.type == type);
            
            buttonToDisable.button.interactable = true;
            buttonToDisable.button.GetComponent<Image>().DOFade(255, 0.8f);
        }
        
        //Enable button on enemy turn
        private void DisableButton() {
            foreach (var powerUp in powerUpButtons)
            {
                if (powerUp.button.interactable == false) return;
                powerUp.button.interactable = false;
                powerUp.button.GetComponent<Image>().DOFade(155, 0.8f);
            }
        }

        private void DisableButton(PowerUpType type)
        {
            var buttonToDisable = powerUpButtons.Single(button => button.type == type);
            
            buttonToDisable.button.interactable = false;
            buttonToDisable.button.GetComponent<Image>().DOFade(155, 0.8f);
        }
        
        private void UpdateCoins(PowerUpType type)
        {
            var cost = powerUpButtons.Single(button => button.type == type);
            this.SendMessage(EventType.OnPlayerCoinReduce, cost.cost);
        }

        private bool IsEnoughMoney(PowerUpType type)
        {
            var cost = powerUpButtons.Single(button => button.type == type);

            return playerData.coins >= cost.cost;
        }

        #endregion

        #region ButtonOnClickEvents
        public void Health()
        {
            if (!IsEnoughMoney(PowerUpType.Health)) return;
            
            //this.SendMessage(EventType.PowerupHealth);
            
            DisableButton(PowerUpType.Health);
            UpdateCoins(PowerUpType.Health);
        }

        public void DamageBuff() {
            if (!IsEnoughMoney(PowerUpType.DamageBuff)) return;
            
            //this.SendMessage(EventType.PowerupDamageBuff);
            
            DisableButton(PowerUpType.DamageBuff);
            UpdateCoins(PowerUpType.DamageBuff);
        }
        
        #endregion
    }
}
