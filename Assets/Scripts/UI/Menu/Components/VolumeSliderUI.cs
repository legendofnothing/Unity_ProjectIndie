using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Components {
    public class VolumeSliderUI : MonoBehaviour {
        public Slider slider;
        public TextMeshProUGUI valueText;

        [Space] 
        public Image iconSprite;
        public Sprite unmuteSprite;
        public Sprite mutedSprite;

        private Sequence _currTween;
        public void OnSliderChange() {
            valueText.SetText((slider.value * 100f).ToString("0"));
            if (slider.value <= 0) {
                if (iconSprite.sprite != mutedSprite) iconSprite.sprite = mutedSprite;
            }

            else {
                if (iconSprite.sprite != unmuteSprite) iconSprite.sprite = unmuteSprite;
            }
        }
    }
}
