using System;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace _src.Scripts.UI {
    public class UI : MonoBehaviour
    {
        [Header("Coins")] 
        public PlayerData playerData;
        public TextMeshProUGUI coinText;
        
        [Header("Turn Number")]
        public TextMeshProUGUI turnNumber;

        private void Start() {
            this.SubscribeListener(EventType.OnTurnNumberChange, param => SetTurnNumber((int) param));
            
            this.SubscribeListener(EventType.OnPlayerCoinAdd, param => CoinAdd((int) param));
            this.SubscribeListener(EventType.OnPlayerCoinReduce, param => CoinReduce((int) param));

            coinText.text = $"{playerData.coins}";
        }

        private void SetTurnNumber(int number) {
            turnNumber.text = number.ToString();
        }

        private void CoinReduce(int amount)
        {
            playerData.coins -= amount;
            coinText.text = $"x{playerData.coins}";
            
            this.SendMessage(EventType.OnPlayerCoinChange);
        }
        
        private void CoinAdd(int amount)
        {
            playerData.coins += amount;
            coinText.text = $"x{playerData.coins}";
            
            this.SendMessage(EventType.OnPlayerCoinChange);
        }
    }
}
