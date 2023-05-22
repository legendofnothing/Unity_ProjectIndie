using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame {
    public class GameUI : MonoBehaviour {
        [Header("Canvas Related")] 
        public CanvasScaler scaler;

        private void Awake() {
            var ratio = Screen.width / (float)Screen.height;
            if (Mathf.Approximately(ratio, 1080f / 1920f)) return;
            scaler.matchWidthOrHeight = Screen.width / (float)Screen.height;
        }
    }
}
