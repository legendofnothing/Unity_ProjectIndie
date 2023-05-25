using System;
using System.Collections.Generic;
using Scripts.Core;
using TMPro;
using UnityEngine;

namespace UI.Menu.Components {
    public class FPSSelectionUI : MonoBehaviour {

        private Dictionary<int, string> settings = new() {
            { 30, "30 FPS" },
            { 60, "60 FPS" },
            { 727, "Unlimited"}
        };

        public TextMeshProUGUI text;
        private int _currSettingIndex;

        private void Start() {
            var currPreset = PlayerPrefs.HasKey(DataKey.FPS) ? PlayerPrefs.GetInt(DataKey.FPS) : 30;
            Application.targetFrameRate = currPreset;
            text.SetText(settings[currPreset]);

            _currSettingIndex = currPreset switch {
                30 => 0,
                60 => 1,
                _ => 2,
            };
        }

        public void SelectPrevious() {
            _currSettingIndex = _currSettingIndex == 0 ? 2 : _currSettingIndex - 1;

            switch (_currSettingIndex) {
                case 0:
                    Application.targetFrameRate = 30;
                    text.SetText(settings[30]);
                    PlayerPrefs.SetInt(DataKey.FPS, 30);
                    break;
                
                case 1:
                    Application.targetFrameRate = 60;
                    text.SetText(settings[60]);
                    PlayerPrefs.SetInt(DataKey.FPS, 60);
                    break;
                
                default:
                    Application.targetFrameRate = 727;
                    text.SetText(settings[727]);
                    PlayerPrefs.SetInt(DataKey.FPS, 727);
                    break;
            }
        }

        public void SelectForward() {
            _currSettingIndex = _currSettingIndex == 2 ? 0 : _currSettingIndex + 1;

            switch (_currSettingIndex) {
                case 0:
                    Application.targetFrameRate = 30;
                    text.SetText(settings[30]);
                    PlayerPrefs.SetInt(DataKey.FPS, 30);
                    break;
                
                case 1:
                    Application.targetFrameRate = 60;
                    text.SetText(settings[60]);
                    PlayerPrefs.SetInt(DataKey.FPS, 60);
                    break;
                
                default:
                    Application.targetFrameRate = 727;
                    text.SetText(settings[727]);
                    PlayerPrefs.SetInt(DataKey.FPS, 727);
                    break;
            }
            
        }
    }
}
