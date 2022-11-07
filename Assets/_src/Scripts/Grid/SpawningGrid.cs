using UnityEngine;

namespace _src.Scripts.Grid {
    public class SpawningGrid : MonoBehaviour {
        public int width;
        public int height;
        public GameObject gridPrefab;
        public float gridOffset;
        
        private void Awake(){
            GenerateGrid();
        }

        public void GenerateGrid(){
            for (int h = 0; h <= height; h++)
            {
                for (int w = 0; w <= width; w++)
                {
                    var pos = new Vector2(w, h);
                    var gridInst = Instantiate(gridPrefab, pos * gridOffset, Quaternion.identity);
                    gridInst.transform.SetParent(gameObject.transform);
                    gridInst.name = $"Grid {w}x{h}";
                }
            }
        }
    }
}
