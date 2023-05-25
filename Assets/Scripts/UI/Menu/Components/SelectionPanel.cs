using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Components {
    public class SelectionPanel : MonoBehaviour {
        private static class Selection {
            public const string Home = "Home";
            public const string Character = "Character";
            public const string Settings = "Settings";
        }

        [Header("Refs")] 
        public List<RectTransform> buttons;
        public RectTransform handler;

        [Header("Selection")] 
        public List<Canvas> selectionList;

        [Header("Selection Text")] 
        public CanvasGroup selectionLegend;
        public TextMeshProUGUI selectionText;

        [Header("Dither")] 
        public Image dither;

        [Header("Tween Settings")] 
        public float duration = 0.8f;
        public Ease easeType = Ease.OutQuint;

        [Header("Color Settings")] 
        public List<Color> buttonColors;

        private Sequence _currTween;
        private Tween _ditherTween;
        private List<CanvasGroup> _selectionCanvasGroup = new();
        private int _currSelection = 0;

        private void Start() {
            selectionLegend.alpha = 0;

            for (var i = 0; i < buttons.Count; i++) {
                buttons[i].GetComponent<Image>().color = buttonColors[i];
                _selectionCanvasGroup.Add(selectionList[i].gameObject.GetComponent<CanvasGroup>());
                selectionList[i].enabled = false;
                _selectionCanvasGroup[i].alpha = 0;
            }
            
            selectionList[1].enabled = true;
            _selectionCanvasGroup[1].alpha = 1;
            _currSelection = 1;
            
            var color = buttonColors[0];
            dither.color = new Color(color.r, color.g, color.b, dither.color.a);
        }

        public void OnSelection(string selection) {
            var prevSelection = _currSelection;
            _currSelection = selection switch {
                Selection.Character => 0,
                Selection.Home => 1,
                _ => 2
            };
            
            selectionText.SetText(selection);
            
            _ditherTween?.Kill();
            var color = buttonColors[_currSelection];
            _ditherTween = dither.DOColor(new Color(color.r, color.g, color.b, dither.color.a), 0.8f);

            _currTween?.Kill();
            _currTween = DOTween.Sequence();
            _currTween
                .Append(handler
                    .DOLocalMove(buttons[_currSelection].localPosition, duration)
                    .SetEase(easeType))
                .Insert(0f, selectionLegend.DOFade(1, 0.15f).SetEase(Ease.InSine))
                .Insert(0f, _selectionCanvasGroup[prevSelection].DOFade(0, 0.15f).OnComplete(() => selectionList[prevSelection].enabled = false))
                .Insert(0f, _selectionCanvasGroup[_currSelection].DOFade(1, 0.15f).OnStart(() => selectionList[_currSelection].enabled = true))
                .Append(DOVirtual.DelayedCall(0.8f, null))
                .Append(selectionLegend.DOFade(0, 1.4f).SetEase(Ease.InSine));

        }

        private void OnDrawGizmosSelected() {
            for (var i = 0; i < buttons.Count; i++) {
                buttons[i].GetComponent<Image>().color = buttonColors[i];
            }
        }
    }
}
