using System;
using Scripts.Core;
using TMPro;
using UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu {
    public class MenuUI : MonoBehaviour {
        private void Awake() {
            SaveSystem.Init();
        }

        private void Start() {
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
        }

        public void Load() {
            SceneManager.LoadScene("Game1");
        }
    }
}
