using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 2)]
    public class LevelData : ScriptableObject {
        [Header("Shop")] 
        public int shopTurn;

        [Header("Pickup")] 
        public int pickupTurn;
        public int minPickUpSpawnAmount;
        public int maxPickUpSpawnAmount;

        [Header("Enemy Manager")] 
        public int enemyCap;
        public int minEnemySpawnAmount;
        public int maxEnemySpawnAmount;

        [Header("Enemy")] 
        public int scoreAdd;
        [Space] 
        public int coinAdd;
        [Space] 
        public float enemyBaseHP;
        public float enemyBaseDMG;
        [Space] 
        public float enemyHPScale;
    }
}
