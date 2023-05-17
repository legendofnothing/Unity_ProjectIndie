using Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu {
    public class MenuUI : MonoBehaviour {
        public TextMeshProUGUI coinDisplayText;
        
        private void Awake() {
            SaveSystem.Init();
        }

        private void Start() {
            coinDisplayText.text = "x" + SaveSystem.playerData.Coin;
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
