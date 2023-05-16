using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

namespace Scripts.ScriptableObjects {
    [CreateAssetMenu(fileName = "EnemySpawningData", menuName = "EnemyData/SpawningData", order = 3)]
    public class EnemySpawningData: ScriptableObject {
        public List<GlobalDefines.SpawnData> enemyData; 
    }
}