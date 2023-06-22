using System;
using DG.Tweening;
using Scripts.Core;
using TMPro;
using UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Menu {
    /**
     * Please DO NOT read any script within UI.Menu namespace. It's the result of me being lazy and not design an
     * actual system for scalability I just don't have the TIME TO DO SO. 
     */
    
    public class MenuUI : MonoBehaviour {
        public Image opener;
        public GameObject audioManager;
        
        private void Awake() {
            if (FindObjectOfType<AudioManager>() == null) {
                Instantiate(audioManager);
            }
            SaveSystem.Init();
            Time.timeScale = 1;
        }

        private void Start() {
            UIStatic.FireUIEvent(TextUI.Type.Coin, SaveSystem.playerData.Coin);
            opener.GetComponent<Canvas>().enabled = true;

            DOVirtual.DelayedCall(1.2f,
                () => {
                    DOVirtual.Float(1, 0, 1.2f, value => { opener.fillAmount = value; })
                        .OnComplete(() => opener.gameObject.SetActive(false));
                });
        }
    }
}
