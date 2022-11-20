using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using _src.Scripts.Core.EventDispatcher;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

namespace _src.Scripts.Managers
{
    public class PickupManager : MonoBehaviour
    {
        [Header("Pickup Spawners")]
        public GameObject pickupSpawnerPrefab;

        [Header("Configs")] 
        public int minAmountSpawn;
        public int maxAmountSpawn;
        [Header("LevelData")] public LevelData levelData;
        
        private GridManager _gridManager;
        private List<Tile> _spawningTiles;
        private List<GameObject> _pickups;

        private int _height;
        private int _width;

        private void Awake()
        {
            _gridManager = gameObject.GetComponent<GridManager>();
            _spawningTiles = new List<Tile>();
            _pickups = new List<GameObject>();

            _height = _gridManager.height;
            _width = _gridManager.width;
            
            InitSpawningGrid();
        }

        private void Start()
        {
            this.SubscribeListener(EventType.SpawnPickup, _=>SpawnPickups());
        }

        private void Update()
        {
            if (IsAllPickupActive())
            {
                _pickups?.RemoveAll(destroyedPickups => destroyedPickups == null);
            }
        }

        private void InitSpawningGrid()
        {
            for (var h = 0; h < _height; h++)
            {
                for (var w = 0; w < _width; w++)
                {
                    _spawningTiles.Add(_gridManager.tiles[w, h]);
                }
            }
        }

        private void SpawnPickups()
        {
            if (_pickups != null)
            {
                foreach (var pickup in _pickups)
                {
                    Destroy(pickup);
                }
                
                _pickups.Clear();
            }
            
            //Spawns every 3 turn
            if (levelData.turnNumber % 3 != 0) return;

            var randomAmount = UnityRandom.Range(minAmountSpawn, maxAmountSpawn);
            
            var rnd = new Random();
            var randomTileSpawners 
                = _spawningTiles
                    .OrderBy(_ => rnd.Next())
                    .Take(randomAmount)
                    .Where(tile => tile.contains == Contains.None)
                    .ToList();
            
            
            foreach (var spawner in randomTileSpawners)
            {
                var pos = spawner.transform.position;
                var pickupinst = Instantiate(pickupSpawnerPrefab, pos, Quaternion.identity);
                
                _pickups?.Add(pickupinst);
            }
        }

        private bool IsAllPickupActive()
        {
            return _pickups.Count > 0;
        }
    }
}