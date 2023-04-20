using System;
using System.Collections.Generic;
using _src.Scripts.Core;
using Unity.Collections;
using UnityEngine;

namespace _src.Scripts.Managers {
    public class GridManager : Singleton<GridManager> {
        /// <summary>
        /// Manager for grid and tiles 
        /// </summary>
        [Header("GridManager Configs")]
        public int width;
        public int height;
        public GameObject gridPrefab;
        public float gridOffset;

        [Header("Store GridManager")] 
        public Transform gridsParent;
        
        [Header("Debug Only")]
        [ReadOnly]
        public Tile[,] tiles;
        
        private void Awake()
        {
            tiles = new Tile[width, height];
            GenerateGrid();
        }

        /// <summary>
        /// Generates Grid and Init Tiles 
        /// </summary>
        public void GenerateGrid(){
            for (var h = 0; h < height; h++)
            {
                for (var w = 0; w < width; w++)
                {
                    //Spawn Grids 
                    var gridPosition = gridsParent.transform.position;
                    var pos = new Vector2(w * gridOffset, h * gridOffset) + (Vector2) gridPosition;
                    
                    //Spawn Tiles 
                    var gridInst = Instantiate(gridPrefab, pos, Quaternion.identity);
                    gridInst.transform.SetParent(gridsParent);
                    gridInst.name = $"GridManager [{w}:{h}]";
                    
                    tiles[w, h] = gridInst.GetComponent<Tile>();
                    tiles[w, h].Init(w, h, Contains.None);
                }
            }
        }
        
        /// <summary>
        /// Update Contain Type of a specific Tile and its overrides 
        /// </summary>
        public void SetTileContainContent(int newX, int newY, Contains newType) {
            tiles[newX, newY].UpdateTile(newType);
        }

        public void ResetTileContainContent(int oldX, int oldY, Contains defaultType = Contains.None) {
            tiles[oldX, oldY].UpdateTile(defaultType);
        }

        public List<Tile> GetEmptyTiles() {
            List<Tile> list = new();
            for (var h = 0; h < height; h++) {
                for (var w = 0; w < width; w++) {
                    if (tiles[w, h].contains == Contains.None) { list.Add(tiles[w, h]); }
                }
            }
            return list;
        }
    }
    
}
