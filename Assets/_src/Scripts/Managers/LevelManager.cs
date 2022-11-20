using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Managers
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
        private GridManager _gridManager;
        private EnemyManager _enemyManager;

        [Header("LevelData")] public LevelData levelData;

        private Turn _currentTurn;

        private void Awake()
        {
            _gridManager = GetComponentInChildren<GridManager>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_gridManager == null) UnityEngine.Debug.Log($"GridManager null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");

            levelData.turnNumber = 1;
        }

        private void Start() {
            UpdateTurn(Turn.Start);
            
            //Subscribe Events w/ other scripts
            this.SubscribeListener(EventType.SwitchToShooting, _=>UpdateTurn(Turn.Shooting));
            this.SubscribeListener(EventType.SwitchToEnemy, _=>UpdateTurn(Turn.Enemy));
            this.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Player));
        }

        private void UpdateTurn(Turn turn)
        {
            _currentTurn = turn;

            switch (_currentTurn)
            {
                case Turn.Start:
                    _enemyManager.SpawnEnemyRandom(3);
                    UpdateTurn(Turn.Player);
                    break;

                case Turn.Player:
                    levelData.turnNumber++;
                    this.SendMessage(EventType.EnablePlayerInput);
                    break;

                case Turn.Shooting:
                    this.SendMessage(EventType.DisablePlayerInput);
                    break;

                case Turn.Enemy:
                    this.SendMessage(EventType.EnemyTurn);
                    break;

                default:
                    UnityEngine.Debug.LogWarning($"Wrong State at {this}");
                    break;
            }
        }
    }
}
