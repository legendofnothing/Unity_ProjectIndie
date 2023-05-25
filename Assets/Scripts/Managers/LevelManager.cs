using ScriptableObjects;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Managers
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

        [FormerlySerializedAs("_currentTurn")] public Turn currentTurn;
        private bool _canAddTurn;

        private void Awake() {
            SaveSystem.Init();
            _gridManager = GetComponentInChildren<GridManager>();
            _enemyManager = GetComponentInChildren<EnemyManager>();
            
            if (_gridManager == null) UnityEngine.Debug.Log($"GridManager null at {this}");
            if (_enemyManager == null) UnityEngine.Debug.Log($"EnemyManager null at {this}");
        }

        private void Start() {
            UpdateTurn(Turn.Start);
            
            //Subscribe Events w/ other scripts
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToShooting, _=>UpdateTurn(Turn.Shooting));
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToEnemy, _=>UpdateTurn(Turn.Enemy));
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToPlayer, _=>UpdateTurn(Turn.Player));
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToShop, _=>UpdateTurn(Turn.Shop));
            EventDispatcher.instance.SubscribeListener(EventType.SwitchToEnd, _=>UpdateTurn(Turn.End));
        }
        
        /// <summary>
        /// Function to update game turn
        /// </summary>
        /// <param name="turn">Turn to update</param>
        private void UpdateTurn(Turn turn)
        {
            currentTurn = turn;

            switch (currentTurn)
            {
                case Turn.Start:
                    _enemyManager.SpawnEnemyRandom(10);
                    UIStatic.FireUIEvent(TextUI.Type.Turn, SaveSystem.currentLevelData.TurnNumber);
                    //UpdateTurn(Turn.Player);
                    break;

                case Turn.Player:
                    SaveSystem.currentLevelData.TurnNumber++; 
                    
                    UIStatic.FireUIEvent(TextUI.Type.Turn, SaveSystem.currentLevelData.TurnNumber);
                    Player.Player.instance.input.CanInput(true);
                    break;

                case Turn.Shooting:
                    Player.Player.instance.input.CanInput(false);
                    break;

                case Turn.Enemy:
                    EventDispatcher.instance.SendMessage(EventType.EnemyTurn);
                    break;
                
                case Turn.Shop:

                    EventDispatcher.instance.SendMessage(SaveSystem.currentLevelData.TurnNumber % 5 == 0
                        ? EventType.OpenShop
                        : EventType.SwitchToPlayer);
                    break;
                
                case Turn.End:
                    SaveSystem.SaveData(SceneManager.GetActiveScene().name);
                    Player.Player.instance.input.CanInput(false);
                    break;

                default:
                    UnityEngine.Debug.LogWarning($"Wrong State at {this}");
                    break;
            }
        }
    }
}
