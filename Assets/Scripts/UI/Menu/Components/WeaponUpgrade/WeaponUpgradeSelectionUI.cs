using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Menu.Components.WeaponUpgrade {
    public class WeaponUpgradeSelectionUI : MonoBehaviour {
        public List<Canvas> panels = new ();
        private List<CanvasGroup> panelGroups = new();

        [Header("Selection UI")] 
        public List<GameObject> selectionIcons = new();
        public GameObject selectionHandler;
        public CanvasGroup selectionLegend;
        public TextMeshProUGUI selectionText;

        private int _currSelection;
        private Sequence _currSeq;
        
        private void Start() {
            foreach (var p in panels.Select(panel => panel.GetComponent<CanvasGroup>())) {
                p.alpha = 0;
                panelGroups.Add(p);
            }

            panels[0].enabled = true;
            panelGroups[0].alpha = 1;
            selectionLegend.alpha = 0;
        }

        public void ChangeSelection(int dir) {
            var prevSelection = _currSelection;
            _currSelection += dir;
            
            if (_currSelection < 0) _currSelection = selectionIcons.Count - 1;
            else if (_currSelection > selectionIcons.Count - 1) _currSelection = 0;
            
            selectionText.SetText(_currSelection switch {
                0 => "Weapon Upgrades",
                1 => "Weapon Gears",
                _ => "Weapons"
            });

            _currSeq?.Kill();
            _currSeq = DOTween.Sequence();
            _currSeq
                .Append(selectionHandler.transform
                    .DOLocalMoveX(selectionIcons[_currSelection].transform.localPosition.x, 0.2f))
                .Insert(0f, selectionLegend.DOFade(1, 0.15f))
                .Insert(0f, panelGroups[_currSelection].DOFade(1, 0.1f).OnComplete(() => panels[_currSelection].enabled = true))
                .Insert(0f, panelGroups[prevSelection].DOFade(0, 0.1f).OnComplete(() => panels[prevSelection].enabled = false))
                .Append(DOVirtual.DelayedCall(0.8f, null))
                .Append(selectionLegend.DOFade(0, 0.2f));
        }
    }
}
