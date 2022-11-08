using System;
using System.Collections;
using _src.Scripts.Spawner;
using UnityEngine;

namespace _src.Scripts.Grid {
    public class SpawningGrid : MonoBehaviour {
        [Header("Grid Configs")]
        public int width;
        public int height;
        public GameObject gridPrefab;
        public float gridOffset;

        [Header("Spawner Configs")] public GameObject spawnerPrefab;

        [Header("Store Grid")] 
        public Transform gridsParent;
        public Transform spawnersParent;
        
        private SpawnerManager _spawnerManager;

        private void Awake(){
            GenerateGrid();
        }

        //Spawn in Inspector
        public void GenerateGrid(){
            _spawnerManager = gameObject.GetComponent<SpawnerManager>();
            
            for (int h = 0; h <= height; h++)
            {
                for (int w = 0; w <= width; w++)
                {
                    var gridPosition = gameObject.transform.position;
                    var pos = new Vector2(w * gridOffset, h * gridOffset) + (Vector2) gridPosition;
                    
                    //Spawn Grid
                    var gridInst = Instantiate(gridPrefab, pos, Quaternion.identity);
                    gridInst.transform.SetParent(gridsParent);
                    gridInst.name = $"Grid [{w}:{h}]";
                    gridInst.GetComponent<Tile>().Init(w, h);
                    
                    //Spawn Spawners
                    if (h < height - 1) continue;
                    var spawnerInst = Instantiate(spawnerPrefab, new Vector2(pos.x, pos.y), 
                        Quaternion.identity);
                    spawnerInst.transform.SetParent(spawnersParent);
                    
                    var s = spawnerInst.GetComponent<SpawnerBase>();
                    spawnerInst.name = $"Spawner {s.spawnerType}[{w}:{h}]";
                    
                    //Add to list
                    _spawnerManager.AddSpawer(spawnerInst);
                }
            }
        }
    }
}
