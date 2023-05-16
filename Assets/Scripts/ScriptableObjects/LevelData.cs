using UnityEngine;

namespace Scripts.ScriptableObjects {
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 2)]
    public class LevelData : ScriptableObject
    {
        public int turnNumber;
        public int score;
        public int sceneIndex;
    }
}
