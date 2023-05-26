using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Menu.Components {
    public class UpgradeSelection : MonoBehaviour {
        public List<RectTransform> selections;
        public RectTransform handler;

        [Space] 
        public CanvasGroup selectionLegend;
        public TextMeshProUGUI selectionText;

        private int _currSelection;
        private Sequence _currSequence;

        private void Start() {
            handler.transform.localPosition = selections[0].localPosition;
            _currSelection = 0;
            selectionLegend.alpha = 0;
        }

        public void OnSelect(int selection) {
            if (selection == _currSelection) return;

            _currSelection = selection;
            selectionText.SetText(_currSelection == 0 ? "Player" : "Weapons");
            
            _currSequence?.Kill();
            _currSequence = DOTween.Sequence();
            _currSequence
                .Append(handler.DOLocalMove(selections[_currSelection].localPosition, 0.4f).SetEase(Ease.OutQuart))
                .Insert(0,selectionLegend.DOFade(1, 0.22f))
                .Append(DOVirtual.DelayedCall(0.8f, null))
                .Append(selectionLegend.DOFade(0, 0.5f));

        }
    }
}
