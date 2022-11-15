using System;
using _src.Scripts.Bullet;
using _src.Scripts.Player;
using Unity.Collections;
using UnityEngine;

namespace _src.Scripts.Grid
{
    public enum Turn
    {
        Start,
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
        [HideInInspector] public int turnNumber; 

        private void Awake()
        {
            _grid = GetComponentInChildren<Grid>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_grid == null) UnityEngine.Debug.Log($"Grid null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");

            currentTurn = Turn.Start;
        }

        private void Update()
        {
            switch (currentTurn)
            {
                case Turn.Start:
                    playerController.canInput = false;
                    break;
                case Turn.Player:
                    playerController.canInput = true;
                    break;
                case Turn.Shooting:
                    playerController.canInput = false;
                    if (!bulletManager.IsAllBulletsActive()) currentTurn = Turn.Enemy;
                    break;
                case Turn.Enemy:
                    turnNumber++;
                    currentTurn = Turn.Player;
                    break;
            }
        }
    }
}
