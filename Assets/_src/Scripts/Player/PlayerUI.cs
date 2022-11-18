using System;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _src.Scripts.Player {
    public class PlayerUI : MonoBehaviour {
        public TextMeshProUGUI healthText;

        private void Awake() {
            this.SubscribeListener(EventType.OnPlayerHPChange, param => SetPlayerHealth((float) param));
        }

        private void SetPlayerHealth(float amount) {
            healthText.text = $"HP: {amount}";
        }
    }
}
