using System;
using DG.Tweening;
using UnityEngine;

namespace UI.Menu.Components {
    public class IWasBored : MonoBehaviour {
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private AudioSource _source;
        
        private void Start() {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _source = GetComponent<AudioSource>();
            _canvas.enabled = false;
            _canvasGroup.alpha = 0;
        }

        public void Begin() {
            _canvas.enabled = true;
            _canvasGroup.DOFade(1, 0.3f);
            _source.Stop();
            _source.Play();
            DOVirtual.DelayedCall(_source.clip.length, () => {
                _canvasGroup.DOFade(0, 0.3f).OnComplete(() => {
                    _canvas.enabled = false;
                });
            });
        }
    }
}
