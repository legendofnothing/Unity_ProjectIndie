using System;
using _src.Scripts.Spawner;
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
        
        private Tile[,] _tiles;
        
        private void Start()
        {
            _tiles = new Tile[width, height];
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
                    
                    _tiles[w, h] = gridInst.GetComponent<Tile>();
                    _tiles[w, h].Init(w, h);
                }
            }
        }
    }
    
}
