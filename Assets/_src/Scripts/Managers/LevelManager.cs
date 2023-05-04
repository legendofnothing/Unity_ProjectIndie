using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.Managers
{
    /// <summary>
    /// Main Game Manager, communicate between managers 
    /// </summary>
    
    public enum Turn
    {
        Start,
        Player,
        Shooting, //This state is when bullets r firing
        Enemy,
        Shop,
        End
    }

    public class LevelManager : Singleton<LevelManager>
    {
        private GridManager _gridManager;
        private EnemyManager _enemyManager;
        
        [Space]
        public EnemySpawningData enemySpawningData;

        [Space] 
        public PickupBulletSpawningData pickupBulletSpawningData;

        private Turn _currentTurn;
        private bool _canAddTurn;

        private void Awake() {
            SaveSystem.instance.Init();
            _gridManager = GetComponentInChildren<GridManager>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_gridManager == null) UnityEngine.Debug.Log($"GridManager null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");
        }

        private void Start() {
            UpdateTurn(Turn.Start);
            
            //Subscribe Events w/ other scripts
            this.SubscribeListener(EventType.SwitchToShooting, _=>UpdateTurn(Turn.Shooting));
            this.SubscribeListener(EventType.SwitchToEnemy, _=>UpdateTurn(Turn.Enemy));
            this.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Player));
            this.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Shop));
            this.SubscribeListener(EventType.SwitchToEnd, _=>UpdateTurn(Turn.End));
        }
        
        /// <summary>
        /// Function to update game turn
        /// </summary>
        /// <param name="turn">Turn to update</param>
        private void UpdateTurn(Turn turn)
        {
            _currentTurn = turn;

            switch (_currentTurn)
            {
                case Turn.Start:
                    _enemyManager.SpawnEnemyRandom(3);
                    this.SendMessage(EventType.OnTurnNumberChange, SaveSystem.instance.currentLevelData.TurnNumber);
                    UpdateTurn(Turn.Player);
                    break;

                case Turn.Player:
                    if (!_canAddTurn) _canAddTurn = true;
                    else SaveSystem.instance.currentLevelData.TurnNumber++; 
                    
                    this.SendMessage(EventType.OnTurnNumberChange, SaveSystem.instance.currentLevelData.TurnNumber);
                    Player.Player.instance.input.CanInput(true);
                    break;

                case Turn.Shooting:
                    Player.Player.instance.input.CanInput(false);
                    break;

                case Turn.Enemy:
                    this.SendMessage(EventType.EnemyTurn);
                    break;
                
                case Turn.Shop:
                    if (SaveSystem.instance.currentLevelData.TurnNumber % 3 != 0) {
                        this.SendMessage(EventType.SwitchToPlayer);
                    }
                    
                    break;
                
                case Turn.End:
                    SaveSystem.instance.SaveData(SceneManager.GetActiveScene().name);
                    Player.Player.instance.input.CanInput(false);
                    this.SendMessage(EventType.OnPlayerDie);
                    break;

                default:
                    UnityEngine.Debug.LogWarning($"Wrong State at {this}");
                    break;
            }
        }
    }
}
