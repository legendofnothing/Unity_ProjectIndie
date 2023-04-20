using System;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.Collections;
using UnityEngine;

using _src.Scripts.Core.EventDispatcher;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

namespace _src.Scripts.Managers
{
    /// <summary>
    /// Manager to handle pickups 
    /// </summary>
    public class PickupManager : MonoBehaviour
    {
        [Header("Pickup Spawners")]
        private readonly WeightedList<GlobalDefines.SpawnData> _weightedPickUpList = new();

        [Header("Configs")] 
        public int minAmountSpawn;
        public int maxAmountSpawn;

        private GridManager _gridManager;
        
        private List<Tile> _spawningTiles;
        
        //List to hold current pickups in the scene 
        private List<GameObject> _pickups;

        private int _height;
        private int _width;

        private void Awake() {
            _gridManager = gameObject.GetComponent<GridManager>();
            _spawningTiles = new List<Tile>();
            _pickups = new List<GameObject>();

            _height = _gridManager.height;
            _width = _gridManager.width;
            
            InitSpawningGrid();

            foreach (var pickup in LevelManager.instance.pickupBulletSpawningData.pickupData) {
                _weightedPickUpList.AddElement(pickup, pickup.chance);
            }
        }

        private void Start() {
            this.SubscribeListener(EventType.SpawnPickup, _=>SpawnPickups());
        }

        private void Update() {
            if (IsAllPickupActive())
            {
                _pickups?.RemoveAll(destroyedPickups => destroyedPickups == null);
            }
        }
        
        /// <summary>
        /// Assign pickup spawning tiles 
        /// </summary>
        private void InitSpawningGrid() {
            for (var h = 0; h < _height; h++)
            {
                for (var w = 0; w < _width; w++)
                {
                    _spawningTiles.Add(_gridManager.tiles[w, h]);
                }
            }
        }
        
        /// <summary>
        /// Spawn pickups 
        /// </summary>
        private void SpawnPickups() {
            //Check if any pickups has been spawned, destroy if have.
            //Pickups only have lifespan of 1 turn.
            if (_pickups != null) {
                foreach (var pickup in _pickups) {
                    Destroy(pickup);
                }
                
                _pickups.Clear();
            }
            
            //Spawns every 3 turn
            if (LevelManager.instance.levelData.turnNumber % 3 != 0) return;

            var randomAmount = UnityRandom.Range(minAmountSpawn, maxAmountSpawn);
            
            //Pick amount of Tiles w/ no duplicates, where the Tile doesn't contain anything
            var rnd = new Random();
            var randomTileSpawners 
                = _spawningTiles
                    .OrderBy(_ => rnd.Next())
                    .Take(randomAmount)
                    .Where(tile => tile.contains == Contains.None)
                    .ToList();
            
            //Spawn pickup on the picked Tiles
            foreach (var spawner in randomTileSpawners) {
                var pos = spawner.transform.position;
                var rndPickup = _weightedPickUpList.GetRandomItem().prefab;
                var pickupinst = Instantiate(rndPickup, pos, Quaternion.identity);
                
                _pickups?.Add(pickupinst);
            }
        }
        
        /// <summary>
        /// Check if any pickups are in the scene 
        /// </summary>
        /// <returns>True if any</returns>
        private bool IsAllPickupActive() {
            return _pickups.Count > 0;
        }
    }
}