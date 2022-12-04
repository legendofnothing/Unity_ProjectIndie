using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.Managers
{
    public enum Turn
    {
        Start,
        Player,
        Shooting, //This state is when bullets r firing
        Enemy,
        End
    }

    public class LevelManager : Singleton<LevelManager>
    {
        private GridManager _gridManager;
        private EnemyManager _enemyManager;

        [Header("LevelData")] 
        public LevelData levelData;
        public LevelData preservedLevelData;

        private Turn _currentTurn;

        private bool _canAddTurn;

        private void Awake()
        {
            _gridManager = GetComponentInChildren<GridManager>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_gridManager == null) UnityEngine.Debug.Log($"GridManager null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");

            levelData.turnNumber = 1;
            levelData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        }

        private void Start() {
            UpdateTurn(Turn.Start);
            
            //Subscribe Events w/ other scripts
            this.SubscribeListener(EventType.SwitchToShooting, _=>UpdateTurn(Turn.Shooting));
            this.SubscribeListener(EventType.SwitchToEnemy, _=>UpdateTurn(Turn.Enemy));
            this.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Player));
            this.SubscribeListener(EventType.SwitchToEnd, _=>UpdateTurn(Turn.End));
        }

        private void UpdateTurn(Turn turn)
        {
            _currentTurn = turn;

            switch (_currentTurn)
            {
                case Turn.Start:
                    _enemyManager.SpawnEnemyRandom(3);
                    this.SendMessage(EventType.OnTurnNumberChange, levelData.turnNumber);
                    UpdateTurn(Turn.Player);
                    break;

                case Turn.Player:
                    if (!_canAddTurn) _canAddTurn = true;
                    else levelData.turnNumber++; 
                    
                    this.SendMessage(EventType.OnTurnNumberChange, levelData.turnNumber);
                    this.SendMessage(EventType.EnablePlayerInput);
                    break;

                case Turn.Shooting:
                    this.SendMessage(EventType.DisablePlayerInput);
                    break;

                case Turn.Enemy:
                    this.SendMessage(EventType.EnemyTurn);
                    break;
                
                case Turn.End:
                    preservedLevelData.turnNumber = levelData.turnNumber;
                    preservedLevelData.score = levelData.score;
                    preservedLevelData.sceneIndex = levelData.sceneIndex;
                    
                    this.SendMessage(EventType.DisablePlayerInput);
                    this.SendMessage(EventType.OnPlayerDie);
                    break;

                default:
                    UnityEngine.Debug.LogWarning($"Wrong State at {this}");
                    break;
            }
        }
    }
}
