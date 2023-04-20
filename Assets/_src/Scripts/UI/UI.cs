using System;
using System.Collections;
using System.Collections.Generic;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.ScriptableObjects;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.UI {
    public class UI : MonoBehaviour
    {
        [Header("Coins")]
        public TextMeshProUGUI coinText;
        
        [Header("Texts")]
        public TextMeshProUGUI turnNumber;
        public TextMeshProUGUI score;

        [Header("UIs")]
        public GameObject gameUI;
        public GameObject pauseUI;

        private void Start() {
            this.SubscribeListener(EventType.OnTurnNumberChange, param => SetTurnNumber((int) param));
            
            this.SubscribeListener(EventType.OnPlayerCoinAdd, param => CoinAdd((int) param));
            this.SubscribeListener(EventType.OnPlayerCoinReduce, param => CoinReduce((int) param));
            
            this.SubscribeListener(EventType.OnPlayerDie, _=>StartCoroutine(EnableDeathUI()));
            
            this.SubscribeListener(EventType.OnScoreChange, param=>SetScore((int) param));

            coinText.text = $"x{SaveSystem.instance.playerData.Coin}";
            score.text = "Score: 0";
            
            ChangeUI(gameUI);
            ChangeUI(null, pauseUI);
        }

        private void SetTurnNumber(int number) {
            turnNumber.text = number.ToString();
        }

        private void CoinReduce(int amount) {
            SaveSystem.instance.playerData.Coin -= amount;
            coinText.text = $"x{SaveSystem.instance.playerData.Coin}";
            
            this.SendMessage(EventType.OnPlayerCoinChange);
        }
        
        private void CoinAdd(int amount) {
            SaveSystem.instance.playerData.Coin += amount;
            coinText.text = $"x{SaveSystem.instance.playerData.Coin}";
            
            this.SendMessage(EventType.OnPlayerCoinChange);
        }

        private void SetScore(int amount) {
            score.text = $"Score: {amount}";
        }

        private IEnumerator EnableDeathUI() {
            var tweenList = new List<Tween>();
            yield return new WaitUntil(() => DOTween.PlayingTweens(tweenList) == null);
            SceneManager.LoadScene("DeathScene");
        }
        
        #region Static Methods
        
        private static void ChangeUI(GameObject newUI = null, GameObject oldUI = null)
        {
            if (newUI != null) newUI.SetActive(true);
            if (oldUI != null) oldUI.SetActive(false);
        }

        #endregion

        #region Buttons

        public void Pause() {
            Time.timeScale = 0;
            ChangeUI(pauseUI, gameUI);
        }

        public void Return() {
            Time.timeScale = 1;
            ChangeUI(gameUI, pauseUI);
        }

        public void Quit() {
            Time.timeScale = 1;
            SceneManager.LoadScene("Menu");
        }

        #endregion
    }
}
