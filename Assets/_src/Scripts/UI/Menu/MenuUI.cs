using System;
using System.Collections;
using _src.Scripts.ScriptableObjects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace _src.Scripts.UI.Menu
{
    public class MenuUI : MonoBehaviour
    {
        [Header("PlayerData")] public PlayerData playerData;
        
        [Header("Starter")] 
        public GameObject loadedUI;
        public AnimationClip loadedUIclip;

        [Header("Texts")] public TextMeshProUGUI coinText;

        [Header("UIs")] public GameObject mainUI;
        
        [Space]
        public GameObject settingUI;
        public GameObject configUI;

        private void Start()
        {
            StartCoroutine(StartUI());

            settingUI.SetActive(false);
            configUI.SetActive(false);

            coinText.text = $"x{playerData.coins}";
        }

        private IEnumerator StartUI()
        {
            loadedUI.SetActive(true);
            yield return new WaitForSeconds(loadedUIclip.length + 0.2f);
            loadedUI.SetActive(false);
        }

        #region MainMenu
        
        public void SettingButton()
        {
            ChangeUI(settingUI, mainUI);
        }
        
        #endregion

        #region Settings

        public void ReturnButton()
        {
            ChangeUI(mainUI, settingUI);
        }

        public void ConfigButton()
        {
            ChangeUI(configUI, settingUI);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ReturnButtonConfig()
        {
            ChangeUI(settingUI, configUI);
        }
        
        #endregion

        private static void ChangeUI(GameObject newUI, GameObject oldUI = null)
        {
            newUI.SetActive(true);
            if (oldUI != null) oldUI.SetActive(false);
        }
    }
}
