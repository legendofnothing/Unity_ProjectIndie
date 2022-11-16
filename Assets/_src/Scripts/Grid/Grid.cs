using System;
using Unity.Collections;
using UnityEngine;

namespace _src.Scripts.Grid {
    public class Grid : MonoBehaviour {
        [Header("Grid Configs")]
        public int width;
        public int height;
        public GameObject gridPrefab;
        public float gridOffset;

        [Header("Spawner Configs")] public GameObject spawnerPrefab;

        [Header("Store Grid")] 
        public Transform gridsParent;
        
        [Header("Debug Only")]
        [ReadOnly]
        public Tile[,] tiles;
        
        private void Awake()
        {
            tiles = new Tile[width, height];
            GenerateGrid();
        }

        //Spawn in Inspector
        public void GenerateGrid(){
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    var gridPosition = gameObject.transform.position;
                    var pos = new Vector2(w * gridOffset, h * gridOffset) + (Vector2) gridPosition;
                    
                    //Spawn Grid
                    var gridInst = Instantiate(gridPrefab, pos, Quaternion.identity);
                    gridInst.transform.SetParent(gridsParent);
                    gridInst.name = $"Grid [{w}:{h}]";
                    
                    tiles[w, h] = gridInst.GetComponent<Tile>();
                    tiles[w, h].Init(w, h);
                }
            }
        }
    }
    
}
