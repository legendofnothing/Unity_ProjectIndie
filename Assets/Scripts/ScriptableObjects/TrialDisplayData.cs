using System;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace ScriptableObjects {
    [Serializable]
    public struct TrialInfo {
        public string name;
        public string description;
        [Space]
        public int difficulty;
        public Color diffColor;
    }
    
    [CreateAssetMenu(fileName = "TrialDisplayData", menuName = "TrialDisplayData", order = 10)]
    public class TrialDisplayData: ScriptableObject {
        public List<TrialInfo> trialInfos;
    }
}