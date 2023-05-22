using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame {
    public class GameUI : MonoBehaviour {
        [Header("UI")] 
        public GameObject footerUI;

        public RectTransform canvasRect;
        private CanvasScaler _scaler;
        private Player.Player _player;

        private void Awake() {
            canvasRect = GetComponent<RectTransform>();
            _scaler = GetComponent<CanvasScaler>();
            
            var ratio = Screen.width / (float)Screen.height;
            if (Mathf.Approximately(ratio, 1080f / 1920f)) return;
            _scaler.matchWidthOrHeight = Screen.width / (float)Screen.height;
        }

        private void Start() {
            _player = Player.Player.instance;
            
            //Set footer to desired position
            var pos 
                = _player.playerCamera
                    .WorldToScreenPoint(_player.transform.position);
            pos.z = 0;
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    pos,
                    _player.playerCamera,
                    out var desired
                );

            footerUI.transform.localPosition = desired;
        }
    }
}
