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

namespace Managers {
    public enum EnemySpawnType {
        Default,
        Random,
    }
    
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
                    //if (h < _height - _spawnHeight) continue;
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
        
        private void RemoveEnemy(EnemyBase enemyToRemove) {
            enemies.Remove(enemyToRemove);
        }
        
        public void SpawnEnemyRandom(int amount) {
            var emptyTiles = _gridManager.GetEmptyTiles().FindAll(tile => tile.contains == Contains.None);
            if (emptyTiles.Count <= 0) return;

            var rnd = new Random();

            for (var i = 0; i < amount; i++) {
                var randomEnemy = _weightedEnemyList.GetRandomItem().prefab;
                var enemyBase = randomEnemy.GetComponent<EnemyBase>();

                var randomTile = emptyTiles
                    .OrderBy(_=>rnd.Next())
                    .FirstOrDefault(tile => enemyBase.spawnType == EnemySpawnType.Default
                        ? tile.y == _gridManager.height - 1
                        : tile.y > 0 || tile.y < _gridManager.height - 2);

                emptyTiles.Remove(randomTile);

                if (randomTile == null) continue;
                var enemyInst = Instantiate(
                    randomEnemy
                    , randomTile.transform.position
                    , Quaternion.identity);
                
                enemyBase = enemyInst.GetComponent<EnemyBase>();
                var adjustedEnemyHp = enemyBase.hp * SaveSystem.currentLevelData.TurnNumber;
                enemyBase.Init(randomTile.x, randomTile.y, adjustedEnemyHp);
                enemies.Add(enemyBase);
                _gridManager.SetTileContainContent(randomTile.x, randomTile.y, Contains.Enemy);
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
