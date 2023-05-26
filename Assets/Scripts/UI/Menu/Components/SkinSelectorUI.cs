using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Components {
    public class SkinSelectorUI : MonoBehaviour {
        public List<Sprite> skins;

        [Header("Refs")] 
        public Image skinDisplay;
        public TextMeshProUGUI skinName;

        private int _currIndex;
        private Sequence _currSeq;
        private bool _canSwitch = true;

        private void Start() {
            _currIndex = PlayerPrefs.HasKey(DataKey.PlayerSkin)
                ? 0
                : PlayerPrefs.GetInt(DataKey.PlayerSkin);

            skinDisplay.sprite = skins[_currIndex];
            skinName.SetText(skins[_currIndex].name);
        }

        public void Select(int dir) {
            if (!_canSwitch) return;
            _canSwitch = false;
            _currIndex += dir;
            
            if (_currIndex < 0) _currIndex = skins.Count - 1;
            else if (_currIndex > skins.Count - 1) _currIndex = 0;
            
            _currSeq?.Kill();
            _currSeq = DOTween.Sequence();
            _currSeq
                .Append(skinDisplay.gameObject.transform
                    .DOLocalMoveY(skinDisplay.gameObject.transform.localPosition.y - 10f, 0.8f))
                .Insert(0.2f, skinDisplay.DOFade(0, 0.6f))
                .Insert(0.8f, skinName.DOFade(0, 0.15f))
                .Append(skinDisplay.gameObject.transform
                    .DOLocalMoveY(0, 0.4f)
                    .OnStart(() => {
                        skinDisplay.sprite = skins[_currIndex];
                        skinName.SetText(skins[_currIndex].name);
                        skinDisplay.DOFade(1, 0.2f);
                        skinName.DOFade(1, 0.1f);
                    }))
                .OnComplete(() => _canSwitch = true);


            PlayerPrefs.SetInt(DataKey.PlayerSkin, _currIndex);
        }
    }
}
