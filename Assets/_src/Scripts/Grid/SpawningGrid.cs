using System;
using System.Collections;
using UnityEngine;

namespace _src.Scripts.Grid {
    public class SpawningGrid : MonoBehaviour {
        [Header("Configs")]
        public int width;
        public int height;
        public GameObject gridPrefab;
        public float gridOffset;

        [Header("Store Grid")] public Transform gridsParents;
        
        //Spawn in Inspector
        public void GenerateGrid(){
            for (int h = 0; h <= height; h++)
            {
                for (int w = 0; w <= width; w++)
                {
                    var gridPosition = gameObject.transform.position;
                    var pos = new Vector2(w * gridOffset, h * gridOffset) + (Vector2) gridPosition;
                    
                    var gridInst = Instantiate(gridPrefab, pos, Quaternion.identity);
                    gridInst.transform.SetParent(gridsParents);
                    gridInst.name = $"Grid [{w}:{h}]";
                    gridInst.GetComponent<Tile>().Init(w, h);
                }
            }
        }
    }
}
