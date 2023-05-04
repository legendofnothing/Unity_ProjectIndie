using System;
using _src.Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.UI.Menu {
    public class MenuUI : MonoBehaviour {
        public TextMeshProUGUI coinDisplayText;
        private SaveSystem _saveSystem;
        
        private void Awake() {
            _saveSystem = SaveSystem.instance;
            _saveSystem.Init();
        }

        private void Start() {
            coinDisplayText.text = "x" + _saveSystem.playerData.Coin;
        }

        #region Button Events
        public void StartGame() {
            SceneManager.LoadScene("Game1");
        }

        public void OpenEquipments() {
            
        }

        public void OpenUpgrades() {
            
        }
        #endregion
    }
}
