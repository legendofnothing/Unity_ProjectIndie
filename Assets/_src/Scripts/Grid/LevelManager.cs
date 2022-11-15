using System;
using _src.Scripts.Bullet;
using _src.Scripts.Player;
using Unity.Collections;
using UnityEngine;

namespace _src.Scripts.Grid
{
    public enum Turn
    {
        Player,
        Shooting, //This state is when bullets r firing
        Enemy,
    }

    public class LevelManager : MonoBehaviour
    {
        public PlayerController playerController;
        public BulletManager bulletManager;
        
        private Grid _grid;
        private EnemyManager _enemyManager;

        [HideInInspector] public Turn currentTurn;

        private void Awake()
        {
            _grid = GetComponentInChildren<Grid>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_grid == null) UnityEngine.Debug.Log($"Grid null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");

            currentTurn = Turn.Player;
        }
    }
}
