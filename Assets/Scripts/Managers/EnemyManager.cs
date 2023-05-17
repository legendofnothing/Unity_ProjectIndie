using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Scripts.Core;
using Scripts.Core.Collections;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;

namespace Managers
{
    /// <summary>
    /// Handles all enemies in the scene 
    /// </summary>
    public class EnemyManager : Singleton<EnemyManager> {
        private readonly WeightedList<GlobalDefines.SpawnData> _weightedEnemyList = new(); 
        
        //Empty GameObject to store all enemies in the scene 
        public Transform enemyStore;
        
       [HideInInspector] public List<EnemyBase> enemies;
        private List<Tile> _spawnerTiles;
        private GridManager _gridManager;

        //Private grid width, height
        private int _width;
        private int _height;
        
        private int _spawnHeight; //Number to spawn at like y:7 
        
        private void Awake()
        {
            enemies = new List<EnemyBase>();
            _spawnerTiles = new List<Tile>();
            _gridManager = gameObject.GetComponent<GridManager>();

            _width = _gridManager.width;
            _height = _gridManager.height;
            _spawnHeight = 1;

            InitSpawnerGrids();

            foreach (var enemy in LevelManager.instance.enemySpawningData.enemyData)
            {
                _weightedEnemyList.AddElement(enemy, enemy.chance);
            }
        }
    
        private void Start() {
            //Subscribe Events 
            EventDispatcher.instance.SubscribeListener(EventType.EnemyTurn, _=>EnemyTurn());
            EventDispatcher.instance.SubscribeListener(EventType.EnemyKilled, param=>RemoveEnemy((EnemyBase) param));
            EventDispatcher.instance.SubscribeListener(EventType.SpawnEnemy, param => SpawnEnemyRandom((int) param));
        }

        //Assign the tiles that will be spawning enemy 
        private void InitSpawnerGrids() {
            for (var h = 0; h < _height; h++) {
                for (var w = 0; w < _width; w++) {
                    if (h < _height - _spawnHeight) continue;
                    _spawnerTiles.Add(_gridManager.tiles[w, h]);
                }
            }
        }
        
        #region Dispatcher Events
        /// <summary>
        /// Execute EnemyTurn in EnemyBase foreach enemies in the scene 
        /// </summary>
        private void EnemyTurn() {
            StartCoroutine(SwitchPlayerTurn());
        }
        
        /// <summary>
        /// Remove Enemy from the scene
        /// </summary>
        /// <param name="enemyToRemove"></param>
        private void RemoveEnemy(EnemyBase enemyToRemove) {
            enemies.Remove(enemyToRemove);
        }
        
        /// <summary>
        /// Spawn an amount of enemy
        /// </summary>
        /// <param name="amount">Amount to spawn</param>
        public void SpawnEnemyRandom(int amount) {
            //Capping amount 
            if (amount > _width * _spawnHeight) amount = _width * _spawnHeight;

            //Pick a random amount of Tile to spawn in, no duplicates
            var rnd = new Random();
            var randomTileSpawners 
                = _spawnerTiles
                    .FindAll(tile => tile.contains == Contains.None)
                    .OrderBy(_ => rnd.Next())
                    .Take(amount)
                    .ToList();
            
            //Spawn enemy in each picked tiles
            foreach (var spawner in randomTileSpawners) {
                var x = spawner.x;
                var y = spawner.y;
                var pos = spawner.transform.position;

                var randomEnemy = _weightedEnemyList.GetRandomItem().prefab;
                
                //Spawn Enemy
                var enemyInst = Instantiate(
                    randomEnemy
                    , pos
                    , Quaternion.identity);
                
                enemyInst.transform.SetParent(enemyStore);
                    
                var enemyBase = enemyInst.GetComponent<EnemyBase>();
                
                //Increase enemy health every turn
                var adjustedEnemyHp = enemyBase.hp * SaveSystem.currentLevelData.TurnNumber;
                
                enemyBase.Init(x,y, adjustedEnemyHp);
                
                //Add To list
                enemies.Add(enemyBase);
                
                //Set Tiles Contents 
                _gridManager.SetTileContainContent(x, y, Contains.Enemy);
            }
            
            PickupManager.instance.SpawnPickups();
        }
        
        private IEnumerator SwitchPlayerTurn() {
            yield return new WaitUntil(() => {
                return enemies.FindAll(enemy => enemy.isEnemyDying).Count <= 0;
            });
            
            PickupManager.instance.DestroyPickup();
            
            foreach (var enemy in enemies) { enemy.OnEnemyTurn(); }
            
            yield return new WaitUntil(() => {
                return enemies.FindAll(enemy => enemy.hasFinishedTurn).Count >= enemies.Count;
            });
        
            yield return new WaitForSeconds(0.4f);
            
            var randomNum = RandomUnity.Range(1, _width - 2);
            SpawnEnemyRandom(randomNum);

            yield return new WaitForSeconds(0.8f);
            EventDispatcher.instance.SendMessage(EventType.SwitchToShop);
        }
        #endregion
    }
}
