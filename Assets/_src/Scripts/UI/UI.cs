using System;
using System.Collections;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.ScriptableObjects;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.UI {
    public class UI : MonoBehaviour
    {
        [Header("Coins")] 
        public PlayerData playerData;
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

            coinText.text = $"x{playerData.coins}";
            score.text = "Score: 0";
            
            ChangeUI(gameUI);
            ChangeUI(null, pauseUI);
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

        private void SetScore(int amount)
        {
            score.text = $"Score: {amount}";
        }

        private IEnumerator EnableDeathUI()
        {
            yield return new WaitForSeconds(0.5f);
            Time.timeScale = 0;
            
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
