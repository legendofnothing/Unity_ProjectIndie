using System;
using System.Collections.Generic;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Grid
{
    public class EnemyManager : MonoBehaviour
    {
        private List<EnemyBase> _enemies;
        
        private void Awake()
        {
            _enemies = new List<EnemyBase>();
        }

        public void AddEnemy(GameObject enemy, Vector3 position, int xCord, int yCord)
        {
            var enemyInst = Instantiate(enemy, position, Quaternion.identity);
            enemyInst.GetComponent<EnemyBase>().Init(xCord, yCord);
        }
    }
}
