using System.Collections.Generic;
using _src.Scripts.Managers;
using UnityEngine;

namespace _src.Scripts.ScriptableObjects {
    [CreateAssetMenu(fileName = "EnemySpawningData", menuName = "EnemyData/SpawningData", order = 1)]
    public class EnemySpawningData: ScriptableObject {
        public List<EnemyData> enemyData; 
    }
}