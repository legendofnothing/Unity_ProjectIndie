using System;
using System.Collections.Generic;
using DG.Tweening;
using ScriptableObjects;
using Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.TrialSelection {
    public static class SceneName {
        public const string Outskirts = "Outskirts";
        public const string AbandonedValley = "AbandonedValley";
        public const string GatesOfHell = "GatesOfHell";
        public const string ThisIsIt = "This Is It";
    }
    
    public class TrialSelection : MonoBehaviour {
        public TrialDisplayData trialData;
        public AudioListData trialAmbienceSounds;
        public AudioClip selectionClip;

        [Header("Lists")] 
        public List<Canvas> trialCanvases = new();
        private List<CanvasGroup> trialGroups = new();
        public List<Image> trialSelectionLegends = new();
        public List<Image> difficultyIcons = new();

        [Header("Texts")] 
        public TextMeshProUGUI missionText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI highScoreText;

        [Header("Closer")] 
        public CloserUITrial closers;

        private int _currIndex;
        private bool _isSwitching;
        private bool _hasStarted;

        private void Awake() {
            foreach (var canvas in trialCanvases) {
                var group = canvas.GetComponent<CanvasGroup>();
                trialGroups.Add(group);
                group.alpha = 0;
                canvas.enabled = false;
            }

            foreach (var icon in difficultyIcons) {
                icon.color = Color.clear;
            }
        }

        private void Start() {
            missionText.SetText(trialData.trialInfos[0].name);
            descriptionText.SetText(trialData.trialInfos[0].description);
            trialCanvases[0].enabled = true;
            trialGroups[0].alpha = 1;
            for (var i = 0; i < trialData.trialInfos[0].difficulty; i++) {
                difficultyIcons[i].gameObject.SetActive(true);
                difficultyIcons[i].color = trialData.trialInfos[0].diffColor;
            }
            
            var key = _currIndex switch {
                0 => SceneName.Outskirts,
                1 => SceneName.AbandonedValley,
                2 => SceneName.GatesOfHell,
                _ => SceneName.ThisIsIt
            };
            
            highScoreText
                .SetText(
                    SaveSystem.playerData.LevelData.TryGetValue(key, out var value)
                        ? value.Score.ToString()
                        : "NO DATA");
            
            AudioManager.instance.PlayMusic(trialAmbienceSounds.list[0], true);
        }

        public void StartGame() {
            if (_hasStarted) return;
            _hasStarted = true;
            closers.Close(_currIndex);
        }

        public void Selection(int dir) {
            if (_isSwitching) return;
            _isSwitching = true;
            
            AudioManager.instance.PlayEffect(AudioManager.EffectType.UISelection);
            
            var prevIndex = _currIndex;
            _currIndex += dir;
            
            if (_currIndex < 0) _currIndex = trialCanvases.Count - 1;
            else if (_currIndex > trialCanvases.Count - 1) _currIndex = 0;
            
            trialCanvases[_currIndex].enabled = true;
            trialGroups[_currIndex].alpha = 0;
            
            AudioManager.instance.SwitchMusic(trialAmbienceSounds.list[_currIndex], true);

            var s = DOTween.Sequence();
            s
                .Append(trialGroups[prevIndex].DOFade(0, 0.6f)
                    .OnComplete(() => {
                        trialCanvases[prevIndex].enabled = false;
                    }))
                .Insert(0,trialGroups[_currIndex].DOFade(1, 0.6f))
                
                .Insert(0.1f, missionText.DOFade(0, 0.15f)
                    .OnComplete(() => missionText.SetText(trialData.trialInfos[_currIndex].name)))
                
                .Insert(0.1f, descriptionText.DOFade(0, 0.15f)
                    .OnComplete(() => descriptionText.SetText(trialData.trialInfos[_currIndex].description)))
                
                .Insert(0.1f, highScoreText.DOFade(0, 0.15f)
                    .OnComplete(() => {
                        var key = _currIndex switch {
                            0 => SceneName.Outskirts,
                            1 => SceneName.AbandonedValley,
                            2 => SceneName.GatesOfHell,
                            _ => SceneName.ThisIsIt
                        };

                        highScoreText
                            .SetText(
                                SaveSystem.playerData.LevelData.TryGetValue(key, out var value)
                                    ? value.Score.ToString()
                                    : "NO DATA");
                    }))
                
                .Insert(0.3f, trialSelectionLegends[prevIndex].DOFade(0.1f, 0.15f))
                
                .Insert(0.5f, trialSelectionLegends[_currIndex].DOFade(0.5f, 0.15f))
                
                .Insert(0.5f, missionText.DOFade(1, 0.3f))
                
                .Insert(0.5f, descriptionText.DOFade(1, 0.3f))
                
                .Insert(0.5f, highScoreText.DOFade(1, 0.15f))
                
                .Insert(0.5f, DOVirtual.DelayedCall(0, () => {
                    for (var i = 0; i < difficultyIcons.Count; i++) {
                        difficultyIcons[i]
                            .DOColor(
                                i >= trialData.trialInfos[_currIndex].difficulty
                                    ? Color.clear
                                    : trialData.trialInfos[_currIndex].diffColor, 0.3f);
                    }
                }))
                .OnComplete(() => _isSwitching = false);
        }
        public void DirectSelection(int index) {
            if (_isSwitching || _currIndex == index) return;
            _isSwitching = true;
            var prevIndex = _currIndex;
            _currIndex = index;
            
            if (_currIndex < 0) _currIndex = trialCanvases.Count - 1;
            else if (_currIndex > trialCanvases.Count - 1) _currIndex = 0;
            
            trialCanvases[_currIndex].enabled = true;
            trialGroups[_currIndex].alpha = 0;

            var s = DOTween.Sequence();
            s
                .Append(trialGroups[prevIndex].DOFade(0, 0.6f)
                    .OnComplete(() => {
                        trialCanvases[prevIndex].enabled = false;
                    }))
                .Insert(0,trialGroups[_currIndex].DOFade(1, 0.6f))
                
                .Insert(0.1f, missionText.DOFade(0, 0.15f)
                    .OnComplete(() => missionText.SetText(trialData.trialInfos[_currIndex].name)))
                
                .Insert(0.1f, descriptionText.DOFade(0, 0.15f)
                    .OnComplete(() => descriptionText.SetText(trialData.trialInfos[_currIndex].description)))
                
                .Insert(0.1f, highScoreText.DOFade(0, 0.15f)
                    .OnComplete(() => {
                        var key = _currIndex switch {
                            0 => SceneName.Outskirts,
                            1 => SceneName.AbandonedValley,
                            2 => SceneName.GatesOfHell,
                            _ => SceneName.ThisIsIt
                        };

                        highScoreText
                            .SetText(
                                SaveSystem.playerData.LevelData.TryGetValue(key, out var value)
                                    ? value.Score.ToString()
                                    : "NO DATA");
                    }))
                
                .Insert(0.3f, trialSelectionLegends[prevIndex].DOFade(0.1f, 0.15f))
                
                .Insert(0.5f, trialSelectionLegends[_currIndex].DOFade(0.5f, 0.15f))
                
                .Insert(0.5f, missionText.DOFade(1, 0.3f))
                
                .Insert(0.5f, descriptionText.DOFade(1, 0.3f))
                
                .Insert(0.5f, highScoreText.DOFade(1, 0.15f))
                
                .Insert(0.5f, DOVirtual.DelayedCall(0, () => {
                    for (var i = 0; i < difficultyIcons.Count; i++) {
                        difficultyIcons[i]
                            .DOColor(
                                i >= trialData.trialInfos[_currIndex].difficulty
                                    ? Color.clear
                                    : trialData.trialInfos[_currIndex].diffColor, 0.3f);
                    }
                }))
                .OnComplete(() => _isSwitching = false);
        }
    }
}
