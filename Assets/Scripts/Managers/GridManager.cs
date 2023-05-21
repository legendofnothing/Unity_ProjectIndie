using System.Collections.Generic;
using Scripts.Core;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers {
    public class GridManager : Singleton<GridManager> {
        /// <summary>
        /// Manager for grid and tiles 
        /// </summary>
        [Header("GridManager Configs")]
        public int width;
        public int height;
        public GameObject gridPrefab;
        [FormerlySerializedAs("gridOffset")] public float gridScale;

        [Header("Store GridManager")] 
        public Transform gridsParent;

        [Header("Tile Config")] 
        public Sprite lightTile;
        public Sprite darkTile;

        [Header("Debug Only")] 
        [ReadOnly] public Tile[,] tiles;
        
        private void Awake() {
            tiles = new Tile[width, height];
            GenerateGrid();
        }

        private void GenerateGrid() {
            var currIndex = 0;

            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    currIndex++;
                    //Spawn Grids 
                    var gridPosition = gridsParent.transform.position;
                    var pos = (Vector2) gridPosition + new Vector2(
                                gridScale * (x - (width - 1) / 2f),
                                gridScale * (y - (height - 1) / 2f));
                    
                    //Spawn Tiles 
                    var gridInst = Instantiate(gridPrefab, pos, Quaternion.identity);
                    gridInst.transform.SetParent(gridsParent);
                    gridInst.transform.localScale = Vector2.one * gridScale;
                    gridInst.name = $"GridManager [{x}:{y}]";

                    var spriteToSet = y % 2 == 0 
                        ? currIndex % 2 == 0 ? lightTile : darkTile 
                        : currIndex % 2 == 0 ? darkTile : lightTile;
                        
                    tiles[x, y] = gridInst.GetComponent<Tile>();
                    tiles[x, y].Init(spriteToSet, x, y, Contains.None);
                }
            }
        }
        
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
