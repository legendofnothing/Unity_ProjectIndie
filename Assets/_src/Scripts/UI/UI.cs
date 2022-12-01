using System;
using _src.Scripts.Core.EventDispatcher;
using TMPro;
using UnityEngine;

namespace _src.Scripts.UI {
    public class UI : MonoBehaviour {
        public TextMeshProUGUI turnNumber;

        private void Start() {
            this.SubscribeListener(EventType.OnTurnNumberChange, param => SetTurnNumber((int) param));
        }

        private void SetTurnNumber(int number) {
            turnNumber.text = number.ToString();
        }
    }
}
