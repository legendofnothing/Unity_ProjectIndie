using System;
using Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu {
    public class SettingUI : MonoBehaviour {
        [Header("Value Getter")] 
        public Slider volumeSlider;

        private void Start() {
            volumeSlider.value = SaveSystem.GetDataFloat(DataKey.Volume);
        }

        public void OnVolumeSlider() {
            AudioListener.volume = volumeSlider.value;
            SaveSystem.SaveDataFloat(DataKey.Volume, volumeSlider.value);
        }
    }
}
