using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.Collections;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy;
using _src.Scripts.Core;
using UnityEngine;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;

namespace _src.Scripts.Managers
{
    /// <summary>
    /// Handles all enemies in the scene 
    /// </summary>
    
    public enum EnemyType
    {
        Basic,
        Range
    }
    
    [Serializable]
    public struct EnemyData
    {
        public EnemyType type;
        public GameObject prefab;
            
        //Weight as in spawn chance, not actual enemy weight, that would crash the phone
        public float weight; 
    }
    
    public class EnemyManager : Singleton<EnemyManager> {
        private readonly WeightedList<EnemyData> _weightedEnemyList = new(); 
        
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
                _weightedEnemyList.AddElement(enemy, enemy.weight);
            }
        }
    
        private void Start()
        {
            //Subscribe Events 
            this.SubscribeListener(EventType.EnemyTurn, _=>EnemyTurn());
            this.SubscribeListener(EventType.EnemyKilled, param=>RemoveEnemy((EnemyBase) param));
            this.SubscribeListener(EventType.SpawnEnemy, param => SpawnEnemyRandom((int) param));
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
            foreach (var enemy in enemies) { enemy.OnEnemyTurn(); }
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
                    .OrderBy(x => rnd.Next())
                    .Take(amount)
                    .ToList();
            
            //Spawn enemy in each picked tiles
            foreach (var spawner in randomTileSpawners)
            {
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
                var adjustedEnemyHp = enemyBase.hp;
                if (LevelManager.instance.levelData.turnNumber > 1) 
                    adjustedEnemyHp = enemyBase.hp * LevelManager.instance.levelData.turnNumber;
                
                enemyBase.Init(x,y, adjustedEnemyHp);
                
                //Add To list
                enemies.Add(enemyBase);
                
                //Set Tiles Contents 
                _gridManager.SetTileContainContent(x, y, Contains.Enemy);
            }
            
            this.SendMessage(EventType.SpawnPickup);
        }
        
        private IEnumerator SwitchPlayerTurn() {
            var canSwitch = false;
            while (!canSwitch) {
                var finishedEnemies = enemies.FindAll(enemy => enemy.hasFinishedTurn = true).Count;
                canSwitch = finishedEnemies >= enemies.Count;
            }
        
            yield return new WaitForSeconds(0.4f);
            
            var randomNum = RandomUnity.Range(1, _width - 2);
            SpawnEnemyRandom(randomNum);

            yield return new WaitForSeconds(0.8f);
            this.SendMessage(EventType.SwitchToPlayer);
        }
        #endregion
    }
}
