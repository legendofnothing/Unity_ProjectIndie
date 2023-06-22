using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "AudioList", menuName = "Audio/AudioList", order = 8)]
    public class AudioListData : ScriptableObject {
        public List<AudioClip> list = new();
    }
}