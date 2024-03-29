using System.Collections.Generic;
using System.Linq;
using Pickups.Bullets;
using Scripts.Core;
using Scripts.Core.Collections;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

using Random = System.Random;
using UnityRandom = UnityEngine.Random;

namespace Managers
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
            if (SaveSystem.currentLevelData.TurnNumber % LevelManager.instance.levelData.pickupTurn != 0) return;
            var randomAmount 
                = UnityRandom.Range(LevelManager.instance.levelData.minPickUpSpawnAmount
                    , LevelManager.instance.levelData.maxPickUpSpawnAmount + 1);
            
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