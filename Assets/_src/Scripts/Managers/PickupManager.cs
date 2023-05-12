using System;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core;
using _src.Scripts.Core.Collections;
using UnityEngine;

using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Pickups;
using _src.Scripts.Pickups.Bullets;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

namespace _src.Scripts.Managers
{
    public class PickupInformation {
        public PickupBullet pickup;
        public Tile tile;
    }
    
    public class PickupManager : Singleton<PickupManager>
    {
        [Header("Pickup Spawners")]
        private readonly WeightedList<GlobalDefines.SpawnData> _weightedPickUpList = new();

        [Header("Configs")] 
        public int minAmountSpawn;
        public int maxAmountSpawn;

        private GridManager _gridManager;
        
        private List<Tile> _spawningTiles;
        
        //List to hold current pickups in the scene 
        private List<PickupInformation> _bulletPickups;

        private int _height;
        private int _width;

        private void Awake() {
            _gridManager = gameObject.GetComponent<GridManager>();
            _spawningTiles = new List<Tile>();
            _bulletPickups = new List<PickupInformation>();

            _height = _gridManager.height;
            _width = _gridManager.width;
            
            InitSpawningGrid();

            foreach (var pickup in LevelManager.instance.pickupBulletSpawningData.pickupData) {
                _weightedPickUpList.AddElement(pickup, pickup.chance);
            }
        }

        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.PickupDestroyed, obj => OnPickupDestroy((PickupBullet) obj));
        }

        private void InitSpawningGrid() {
            for (var h = 0; h < _height; h++)
            {
                for (var w = 0; w < _width; w++)
                {
                    _spawningTiles.Add(_gridManager.tiles[w, h]);
                }
            }
        }
        
        public void SpawnPickups() {
            if (SaveSystem.instance.currentLevelData.TurnNumber % 3 != 0) return;
            var randomAmount = UnityRandom.Range(minAmountSpawn, maxAmountSpawn);
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
                var pickupInstance = Instantiate(rndPickup, pos, Quaternion.identity);

                _gridManager.SetTileContainContent(spawner.x, spawner.y, Contains.Pickup);
                
                _bulletPickups?.Add(new PickupInformation {
                    pickup = pickupInstance.GetComponent<PickupBullet>(),
                    tile = spawner
                });
            }
        }

        public void DestroyPickup() {
            if (_bulletPickups == null) return;
            foreach (var pickup in _bulletPickups) {
                pickup.pickup.Destroy();
            }
        }

        private void OnPickupDestroy(PickupBullet pickup) {
           var pickupToDestroy = _bulletPickups.Find(pickupToDestroy => pickupToDestroy.pickup == pickup);
           _gridManager.SetTileContainContent(pickupToDestroy.tile.x, pickupToDestroy.tile.y, Contains.None);
           _bulletPickups.Remove(pickupToDestroy);
           Destroy(pickupToDestroy.pickup);
        }
    }
}