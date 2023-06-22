using System;
using DG.Tweening;
using ScriptableObjects;
using Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers {
    public enum AudioType {
    }
    
    public class AudioManagerHelper : Singleton<AudioManagerHelper> {
        [Header("Sound Track")] 
        public AudioListData levelSoundTrack;
        private int _currentTrackIndex = 0;
        private Sequence _trackSequence;


        public void PlaySoundTrack() {
            _trackSequence = DOTween.Sequence();
            var clipToPlay = levelSoundTrack.list[_currentTrackIndex];
            AudioManager.instance.PlayMusic(clipToPlay);

            _trackSequence
                .Append(DOVirtual.DelayedCall(clipToPlay.length + 5f, null))
                .OnComplete(() => {
                    _currentTrackIndex++;
                    if (_currentTrackIndex >= levelSoundTrack.list.Count) _currentTrackIndex = 0;
                    PlaySoundTrack();
                });
        }
        
    }
}
