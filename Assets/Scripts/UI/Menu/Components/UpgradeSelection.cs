using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Menu.Components {
    public class UpgradeSelection : MonoBehaviour {
        public List<RectTransform> selections;
        public RectTransform handler;

        [Space] 
        public CanvasGroup selectionLegend;
        public TextMeshProUGUI selectionText;

        [Space] public List<CanvasGroup> panels;

        private int _currSelection;
        private Sequence _currSequence;

        private void Start() {
            handler.transform.localPosition = selections[0].localPosition;
            _currSelection = 0;
            selectionLegend.alpha = 0;
            
            panels[0].alpha = 1;
            panels[0].GetComponent<Canvas>().enabled = true;
            
            panels[^1].alpha = 0;
            panels[^1].GetComponent<Canvas>().enabled = false;
        }

        public void OnSelect(int selection) {
            if (selection == _currSelection) return;
            AudioManager.instance.PlayPanelSwitchEffect(Random.Range(0, 2));

            _currSelection = selection;
            var _prevSelection = selection == 0 ? 1 : 0;
            selectionText.SetText(_currSelection == 0 ? "Player" : "Weapons");
            
            panels[_currSelection].GetComponent<Canvas>().enabled = true;
            
            _currSequence?.Kill();
            _currSequence = DOTween.Sequence();
            _currSequence
                .Append(handler.DOLocalMove(selections[_currSelection].localPosition, 0.4f).SetEase(Ease.OutQuart))
                .Insert(0,selectionLegend.DOFade(1, 0.22f))
                .Insert(0, panels[_currSelection].DOFade(1, 0.15f))
                .Insert(0, panels[_prevSelection].DOFade(0, 0.15f).OnComplete(() => panels[_prevSelection].GetComponent<Canvas>().enabled = false))
                .Append(DOVirtual.DelayedCall(0.8f, null))
                .Append(selectionLegend.DOFade(0, 0.5f));

        }
    }
}
