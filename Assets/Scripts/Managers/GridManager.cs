using System.Collections.Generic;
using Scripts.Core;
using Unity.Collections;
using UnityEngine;

namespace Managers {
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

        [Header("Tile Config")] 
        public Sprite lightTile;
        public Sprite darkTile;
        
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
        public void GenerateGrid() {
            var currIndex = 0;

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

                    Sprite spriteToSet;
                    if (h % 2 == 0) spriteToSet = currIndex % 2 == 0 ? lightTile : darkTile;
                    else spriteToSet = currIndex % 2 == 0 ? darkTile : lightTile;
                    
                    tiles[w, h] = gridInst.GetComponent<Tile>();
                    tiles[w, h].Init(spriteToSet, w, h, Contains.None);
                    
                    currIndex++;
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
