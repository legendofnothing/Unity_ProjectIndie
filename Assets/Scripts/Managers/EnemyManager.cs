using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Scripts.Core;
using Scripts.Core.Collections;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.Serialization;
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
        [HideInInspector] public List<EnemyBase> enemies = new ();
        private GridManager _gridManager;
        private LevelManager _levelManager;
        [HideInInspector] public List<Tile> emptyTilesInst = new ();

        private void Awake() {
            _gridManager = gameObject.GetComponent<GridManager>();
            
            foreach (var enemy in LevelManager.instance.enemySpawningData.enemyData) {
                _weightedEnemyList.AddElement(enemy, enemy.chance);
            }
            
            _levelManager = LevelManager.instance;
        }
    
        private void Start() {
            //Subscribe Events 
            EventDispatcher.instance.SubscribeListener(EventType.EnemyTurn, _=>EnemyTurn());
            EventDispatcher.instance.SubscribeListener(EventType.EnemyKilled, param=>RemoveEnemy((EnemyBase) param));
        }
        

        #region Dispatcher Events
        private void EnemyTurn() {
            StartCoroutine(SwitchPlayerTurn());
        }
        
        private void RemoveEnemy(EnemyBase enemyToRemove) {
            enemies.Remove(enemyToRemove);
        }
        
        private IEnumerator SwitchPlayerTurn() {
            yield return new WaitUntil(() => {
                return enemies.FindAll(enemy => enemy.isEnemyDying).Count <= 0;
            });
            
            PickupManager.instance.DestroyPickup();
            yield return new WaitForSeconds(0.4f);
     
            emptyTilesInst = _gridManager.GetEmptyTiles();
            var enemySorted = enemies.OrderByDescending(enemy => enemy.movePriority);
            foreach (var enemy in enemySorted) {
                enemy.OnEnemyTurn();
            }
            
            emptyTilesInst.Clear();
            yield return new WaitUntil(() => {
                return enemies.FindAll(enemy => enemy.hasFinishedTurn).Count >= enemies.Count;
            });
        
            yield return new WaitForSeconds(0.4f);
            
            var randomNum = RandomUnity.Range(
                _levelManager.levelData.minEnemySpawnAmount, _levelManager.levelData.maxEnemySpawnAmount + 1);
            SpawnEnemyRandom(randomNum);

            yield return new WaitForSeconds(0.8f);
            EventDispatcher.instance.SendMessage(EventType.SwitchToShop);
        }
        #endregion

        #region Helper Functions
        public void SpawnEnemyRandom(int amount) {
            var emptyTiles = _gridManager.GetEmptyTiles();
            if (emptyTiles.Count <= 0 && enemies.Count >= _levelManager.levelData.enemyCap) return;

            if (enemies.Count + amount >= _levelManager.levelData.enemyCap) {
                amount = _levelManager.levelData.enemyCap - enemies.Count;
            }
            
            var rnd = new Random();

            for (var i = 0; i < amount; i++) {
                var randomEnemy = _weightedEnemyList.GetRandomItem().prefab;
                var enemyBase = randomEnemy.GetComponent<EnemyBase>();

                var randomTile = emptyTiles
                    .OrderBy(_=>rnd.Next())
                    .FirstOrDefault(tile => enemyBase.spawnType == EnemySpawnType.Default
                        ? tile.y == _gridManager.height - 1
                        : tile.y > 0 && tile.y < _gridManager.height - 2);

                emptyTiles.Remove(randomTile);

                if (randomTile == null) continue;
                var enemyInst = Instantiate(
                    randomEnemy
                    , randomTile.transform.position
                    , Quaternion.identity);
                
                enemyBase = enemyInst.GetComponent<EnemyBase>();
                enemyBase.Init(randomTile.x, randomTile.y);
                enemies.Add(enemyBase);
                _gridManager.SetTileContainContent(randomTile.x, randomTile.y, Contains.Enemy);
            }

            PickupManager.instance.SpawnPickups();
        }
        
        #endregion
        
    }
}
