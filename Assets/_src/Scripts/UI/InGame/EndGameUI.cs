using System;
using System.Collections;
using _src.Scripts.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.UI.InGame {
    public class EndGameUI : MonoBehaviour {
        private Animator _animator;
        
        private void Start() {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void TransitToDeathScene() {
            DOVirtual.Float(1, 0, 1.5f, value => {
                Time.timeScale = value;

                if (value <= 0.1f) {
                    _animator.SetBool("Init", true);
                }
            }).SetUpdate(true);
        }

        public void OnInitFinished() {
            DOTween.KillAll();
            SceneManager.LoadScene("DeathScene");
        }
    }
}
