using System.Collections.Generic;
using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.ScriptableObjects {
    [CreateAssetMenu(fileName = "EnemySpawningData", menuName = "EnemyData/SpawningData", order = 3)]
    public class EnemySpawningData: ScriptableObject {
        public List<GlobalDefines.SpawnData> enemyData; 
    }
}