using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Player;
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
        private Grid _grid;
        private EnemyManager _enemyManager;
        
        [Header("Debug Only")]
        public Turn currentTurn;
        public int turnNumber = 1; 

        private void Awake()
        {
            _grid = GetComponentInChildren<Grid>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_grid == null) UnityEngine.Debug.Log($"Grid null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");
        }

        private void Start() {
            turnNumber = 1;
            UpdateTurn(Turn.Start);
            
            //Subscribe Events w/ other scripts
            this.SubscribeListener(EventType.SwitchToShooting, _=>UpdateTurn(Turn.Shooting));
            this.SubscribeListener(EventType.SwitchToEnemy, _=>UpdateTurn(Turn.Enemy));
            this.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Player));
        }

        public void UpdateTurn(Turn turn)
        {
            currentTurn = turn;

            switch (currentTurn)
            {
                case Turn.Start:
                    _enemyManager.SpawnEnemyRandom(3);
                    UpdateTurn(Turn.Player);
                    break;

                case Turn.Player:
                    this.SendMessage(EventType.EnablePlayerInput);
                    break;

                case Turn.Shooting:
                    this.SendMessage(EventType.DisablePlayerInput);
                    break;

                case Turn.Enemy:
                    UpdateTurn(Turn.Player);
                    break;

                default:
                    UnityEngine.Debug.LogWarning($"Wrong State at {this}");
                    break;
            }
        }
    }
}
