using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class TitleScreen : MonoBehaviour {
        public List<CanvasGroup> groups = new();

        private void Start() {
            foreach (var group in groups) {
                group.alpha = 0;
            }

            var s = DOTween.Sequence();

            s
                .Append(groups[0].DOFade(1, 0.8f))
                .AppendInterval(1.4f)
                .Append(groups[0].DOFade(0, 0.2f))
                .Append(groups[1].DOFade(1, 0.8f))
                .AppendInterval(1.4f)
                .Append(groups[1].DOFade(0, 0.2f))
                .Append(groups[2].DOFade(1, 0.8f))
                .AppendInterval(2f)
                .Append(groups[2].DOFade(0, 0.2f))
                .OnComplete(() => {
                    SceneManager.LoadScene("Scenes/Menu");
                });
        }
    }
}
