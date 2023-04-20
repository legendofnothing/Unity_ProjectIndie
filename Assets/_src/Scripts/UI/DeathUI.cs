using System;
using _src.Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.UI
{
    public class DeathUI : MonoBehaviour
    {
        [Header("Text")] 
        public TextMeshProUGUI highScoreNum;
        public TextMeshProUGUI turnNum;

        private void Awake() {
            SaveSystem.instance.Init();
        }

        private void Start() {
            Time.timeScale = 0;
            gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

            var data = SaveSystem.instance.playerData;
            highScoreNum.text = $"{data.LevelData[data.PreviousSceneName].Score}";
            turnNum.text = $"{data.LevelData[data.PreviousSceneName].TurnNumber}";
        }

        public void QuitToMenu() {
            SceneManager.LoadScene("Menu");
            Time.timeScale = 1;
        }

        public void Retry() {
            SceneManager.LoadScene(SaveSystem.instance.playerData.PreviousSceneName);
            Time.timeScale = 1;
        }
    }
}
