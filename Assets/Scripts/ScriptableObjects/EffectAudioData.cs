using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "EffectList", menuName = "Audio/EffectList", order = 9)]
    public class EffectAudioData : ScriptableObject {
        public List<AudioManager.Effect> effects = new();
    }
}