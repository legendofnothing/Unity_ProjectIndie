using System;
using System.Collections.Generic;
using DG.Tweening;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour {
    public static AudioManager instance;
    public enum EffectType {
        UISelection,
        UISkinSelection,
        UIPanelSwitch1,
        UIPanelSwitch2,
        UIGunEquip,
        UIBuySuccess,
        UIBuyReject
    }

    public enum MasterOption {
        Mute,
        Unmute,
    }
    
    [Serializable]
    public struct Effect {
        public EffectType type;
        public AudioClip clip;
    }

    public static class MixerParam {
        public const string MusicLowPass = "MusicLowPass";
        public const string MasterVolume = "MasterVolume";
    }

    [Header("Config")] 
    public float muffledThreshold;
    
    [Header("Refs")]
    public AudioMixer mixer;
    public EffectAudioData effectsData;
    [Space]
    public List<AudioSource> musicSources;
    public AudioSource sfxSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else Destroy(this);
        
        mixer.SetFloat(MixerParam.MasterVolume, 0);
        mixer.SetFloat(MixerParam.MusicLowPass, 22000);
    }

    private void OnDestroy() {
        DOTween.KillAll();
        mixer.SetFloat(MixerParam.MasterVolume, 0);
        mixer.SetFloat(MixerParam.MusicLowPass, 22000);
    }

    public void LowerMusic(MasterOption option, float duration = 1f) {
        var targetValue = option == MasterOption.Mute ? -80 : 0;
        mixer.GetFloat(MixerParam.MasterVolume, out var currValue);
        if (Mathf.Approximately(currValue, targetValue)) return;
        DOVirtual.Float(currValue, targetValue, duration, value => {
            mixer.SetFloat(MixerParam.MasterVolume, value);
        }).SetUpdate(true);
    }

    #region Music Methods
    public void PlayMusic(AudioClip clip, bool isLoop = false) {
        var source = GetMusicSource;
        source.volume = 1;
        source.loop = isLoop;
        source.clip = clip;
        source.Play();
    }

    public void StopMusic() {
        foreach (var musicSource in musicSources) {
            musicSource.Stop();
        }
    }
    
    private AudioSource GetMusicSource => musicSources.Find(source => !source.isPlaying);
    
    public void SwitchMusic(AudioClip clip, bool isLoop = false) {
        var playingSource = musicSources.Find(source => source.isPlaying);
        var freeSource = musicSources.Find(source => !source.isPlaying);

        freeSource.volume = 0;
        freeSource.clip = clip;
        freeSource.loop = isLoop;
        freeSource.Play();

        DOVirtual.Float(0, 1, 1.5f, value => {
            freeSource.volume = value;
            playingSource.volume = 1 - value;
        }).OnComplete(() => {
            playingSource.Stop();
            playingSource.volume = 1;
        }).SetUpdate(true);
    }

    public void MuffleMusic(bool isUnMuffle = false, float duration = 1f) {
        var targetValue = isUnMuffle ? 22000f : muffledThreshold;
        mixer.GetFloat(MixerParam.MusicLowPass, out var currValue);
        
        if (Mathf.Approximately(currValue, targetValue)) return;
        DOVirtual.Float(currValue, targetValue, duration, value => {
            mixer.SetFloat(MixerParam.MusicLowPass, value);
        }).SetUpdate(true);
    }

    #endregion

    #region Effect Methods

    public void PlayEffect(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayEffect(EffectType type) {
        var effect = effectsData.effects.Find(e => e.type == type);
        sfxSource.PlayOneShot(effect.clip);
    }

    public void PlayPanelSwitchEffect(int type) {
        var typeToFind = type switch {
            0 => EffectType.UIPanelSwitch1,
            _ => EffectType.UIPanelSwitch2
        };
        
        PlayEffect(typeToFind);
    }

    #endregion
}
