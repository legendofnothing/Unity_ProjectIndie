using System;
using DG.Tweening;
using ScriptableObjects;
using Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers {
    public enum AudioType {
        PLAYER_Hurt,
        PLAYER_Death,
        PLAYER_Impact,
        
        ENEMY_Impact,
        ENEMY_Death,
        
        TURN_Indicator,
    }
    
    public class AudioManagerHelper : Singleton<AudioManagerHelper> {
        [Header("Sound Track")] 
        public AudioListData levelSoundTrack;
        private int _currentTrackIndex = 0;
        private Sequence _trackSequence;

        [Header("Sound Effect")] 
        public AudioListData playerHurtAudio;
        public AudioListData playerDeathAudio;
        public AudioListData playerImpactAudio;
        [Space] 
        public AudioListData enemyImpactSound;
        public AudioListData enemyDeathSound;
        [Space] 
        public AudioListData turnIndicatorSound;

        public void PlayEffect(AudioType type) {
            var clip = GetClipFromType(type);
            AudioManager.instance.PlayEffect(clip);
        }

        private AudioClip GetClipFromType(AudioType type) {
            var list = type switch {
                AudioType.PLAYER_Hurt => playerHurtAudio.list,
                AudioType.PLAYER_Death => playerDeathAudio.list,
                AudioType.PLAYER_Impact => playerImpactAudio.list,
                AudioType.ENEMY_Impact => enemyImpactSound.list,
                AudioType.ENEMY_Death => enemyDeathSound.list,
                AudioType.TURN_Indicator => turnIndicatorSound.list,
                _ => null
            };

            return list[Random.Range(0, list.Count)];
        }

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
