using System;
using DG.Tweening;
using Managers;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using TMPro;
using UI.Components;
using UI.InGame.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame {
    public class GameUI : MonoBehaviour {
        [Header("UI")] 
        public GameObject footerUI;
        public RectTransform canvasRect;

        [Header("Pause")]
        public Canvas pauseCanvas;
        public CanvasGroup pauseGroup;

        [Header("Timescale Button")] 
        public TextMeshProUGUI timeScaleText;
        public Sprite defaultTimescaleSprite;
        public Sprite ffdTimescaleSprite;
        public Image timeScaleImage;

        [Header("Closer")] 
        public CloserUI closerUI;

        private CanvasScaler _scaler;
        private Player.Player _player;

        private bool _isPaused;
        private bool _hasReturned;
        [HideInInspector] public float currTimeScale = 1f;

        private void Awake() {
            canvasRect = GetComponent<RectTransform>();
            _scaler = GetComponent<CanvasScaler>();
            
            var ratio = Screen.width / (float)Screen.height;
            if (Mathf.Approximately(ratio, 1080f / 1920f)) return;
            _scaler.matchWidthOrHeight = Screen.width / (float)Screen.height;
        }

        private void Start() {
            _player = Player.Player.instance;
            timeScaleText.SetText("x1");
            
            pauseGroup.alpha = 0;
            pauseCanvas.enabled = false;
            
            var ratio = Screen.width / (float)Screen.height;
            
            //Set footer to desired position
            var pos
                = ratio > 0.5f
                    ? _player.playerCamera.WorldToScreenPoint(_player.transform.position)
                    : _player.playerCamera.WorldToScreenPoint(_player.transform.position - new Vector3(0, 0.9f, 0));
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

        #region Button Events

        public void PauseGame() {
            if (_isPaused) return;
            _isPaused = true;
            if (LevelManager.instance.currentTurn == Turn.Player) Player.Player.instance.input.CanInput(false);
            
            pauseCanvas.gameObject.SetActive(true);
            pauseGroup.alpha = 0;
            pauseCanvas.enabled = true;
            
            EventDispatcher.instance.SendMessage(EventType.OnDimUI, new DimUI.Message {
                isOpen = true,
                duration = 0.15f
            });
            
            AudioManager.instance.MuffleMusic(false, 0.2f);

            var s = DOTween.Sequence();
            s
                .Append(pauseGroup.DOFade(1, 0.15f).SetUpdate(true))
                .Insert(0, DOVirtual.Float(currTimeScale, 0, 0.15f, value => Time.timeScale = value).SetUpdate(true));
        }

        public void SwitchTimeScale() {
            Time.timeScale = currTimeScale is >= 1f and < 2 ? 2 : 1;
            timeScaleImage.sprite = currTimeScale is >= 1f and < 2 ? ffdTimescaleSprite : defaultTimescaleSprite;
            currTimeScale = Time.timeScale;
            timeScaleText.SetText("x" + currTimeScale.ToString("0"));
            
        }

        public void UnPause() {
            if (!_isPaused) return;
            _isPaused = false;

            EventDispatcher.instance.SendMessage(EventType.OnDimUI, new DimUI.Message {
                isOpen = false,
                duration = 0.1f
            });
            
            AudioManager.instance.MuffleMusic(true, 0.2f);

            var s = DOTween.Sequence();
            s
                .Append(pauseGroup.DOFade(0, 0.1f).SetUpdate(true))
                .Insert(0, DOVirtual.Float(0, currTimeScale, 0.1f, value => Time.timeScale = value).SetUpdate(true))
                .OnComplete(() => {
                    pauseCanvas.gameObject.SetActive(false);
                    pauseGroup.alpha = 0;
                    pauseCanvas.enabled = false;
                    if (LevelManager.instance.currentTurn == Turn.Player) Player.Player.instance.input.CanInput(true);
                });
        }
        
        public void Return() {
            if (_hasReturned) return;
            _hasReturned = true;
            closerUI.Close(CloserUI.CloserType.ReturnToMenu);
        }
        
        public void ExitGame() {
            if (_hasReturned) return;
            _hasReturned = true;
            SaveSystem.SaveData(SceneManager.GetActiveScene().name);
            closerUI.Close(CloserUI.CloserType.ExitGame);
        }

        #endregion
    }
}
