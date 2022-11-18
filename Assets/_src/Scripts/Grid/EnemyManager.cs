using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy;
using UnityEngine;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;

namespace _src.Scripts.Grid
{
    public class EnemyManager : MonoBehaviour
    {
        public Transform enemyStore;
        public GameObject enemyPrefab;
        
        private List<EnemyBase> _enemies;
        private List<Tile> _spawnerTiles;
        private Grid _grid;

        private int _width;
        private int _height;
        private int _spawnHeight; //Number to spawn at like y:7 

        private void Awake()
        {
            _enemies = new List<EnemyBase>();
            _spawnerTiles = new List<Tile>();
            _grid = gameObject.GetComponent<Grid>();

            _width = _grid.width;
            _height = _grid.height;
            _spawnHeight = 1;
            
            InitSpawnerGrids();
        }

        private void Start()
        {
            //Subscribe Event
            this.SubscribeListener(EventType.EnemyTurn, _=>EnemyTurn());
            this.SubscribeListener(EventType.EnemyKilled, param=>RemoveEnemy((EnemyBase) param));
            this.SubscribeListener(EventType.SpawnEnemy, param => SpawnEnemyRandom((int) param));
        }

        //Assign the tiles that will be spawning enemy 
        private void InitSpawnerGrids()
        {
            for (var h = 0; h < _height; h++)
            {
                for (var w = 0; w < _width; w++)
                {
                    if (h < _height - _spawnHeight) continue;
                    _spawnerTiles.Add(_grid.tiles[w, h]);
                }
            }
        }

        private void AddEnemy(GameObject enemy, Vector3 position, int xCord, int yCord)
        {
            var enemyInst = Instantiate(enemy, position, Quaternion.identity);
            enemyInst.transform.SetParent(enemyStore);
                    
            var enemyBase = enemyInst.GetComponent<EnemyBase>();
            enemyBase.Init(xCord, yCord);
            _enemies.Add(enemyBase);
        }
        
        #region Dispatcher Events
        private void EnemyTurn()
        {
            foreach (var enemy in _enemies)
            {
                int updatedEnemyYCord;
                
                if (enemy.y - 1 <= 0) updatedEnemyYCord = 0;
                else updatedEnemyYCord = enemy.y - 1;
                
                var pos = _grid.tiles[enemy.x, updatedEnemyYCord].transform.position;
                StartCoroutine(enemy.EnemyTurnCoroutine(pos, updatedEnemyYCord));
            }

            StartCoroutine(SwitchPlayerTurn());
        }

        private void RemoveEnemy(EnemyBase enemyToRemove)
        {
            _enemies.Remove(enemyToRemove);
        }
        
        public void SpawnEnemyRandom(int amount)
        {
            if (amount > _width * _spawnHeight) amount = _width * _spawnHeight;
            
            var rnd = new Random();
            var randomTileSpawners 
                = _spawnerTiles.OrderBy(x => rnd.Next()).Take(amount).ToList();

            foreach (var spawner in randomTileSpawners)
            {
                var x = spawner.x;
                var y = spawner.y;
                var pos = spawner.transform.position;

                AddEnemy(enemyPrefab, pos, x, y);
            }
        }

        private IEnumerator SwitchPlayerTurn()
        {
            yield return new WaitForSeconds(0.8f);
            var randomNum = RandomUnity.Range(1, _width - 1);
            SpawnEnemyRandom(randomNum);

            yield return new WaitForSeconds(0.8f);
            this.SendMessage(EventType.SwitchToPlayer);
        }
        #endregion
    }
}
