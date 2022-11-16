using System;
using System.Collections;
using System.Threading.Tasks;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
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
        Enemy
    }

    public class LevelManager : Singleton<LevelManager>
    {
        public PlayerController playerController;
        public BulletManager bulletManager;
        
        private Grid _grid;
        private EnemyManager _enemyManager;

        [HideInInspector] public Turn currentTurn;
        [HideInInspector] public int turnNumber = 1; 

        private void Awake()
        {
            _grid = GetComponentInChildren<Grid>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_grid == null) UnityEngine.Debug.Log($"Grid null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");
            
            this.SubscribeListener(EventType.SwitchToShooting, _=>UpdateTurn(Turn.Shooting));
            this.SubscribeListener(EventType.SwitchToEnemy, _=>UpdateTurn(Turn.Enemy));
            this.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Player));
        }

        private void Start() {
            turnNumber = 1;
            UpdateTurn(Turn.Start);
        }

        public void UpdateTurn(Turn turn) {
            while (true) {
                currentTurn = turn;

                switch (currentTurn) {
                    case Turn.Start:
                        _enemyManager.SpawnEnemyRandom(3);
                        playerController.enabled = false;
                        bulletManager.enabled = false;
                        turn = Turn.Player;
                        continue;
                    case Turn.Player:
                        playerController.enabled = true;
                        bulletManager.enabled = true;

                        break;
                    case Turn.Shooting:
                        playerController.enabled = false;

                        break;
                    case Turn.Enemy:


                        break;
                    default:
                        UnityEngine.Debug.LogWarning($"Wrong State at {this}");
                        break;
                }

                break;
            }
        }
    }
}
