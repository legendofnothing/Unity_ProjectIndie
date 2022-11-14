using System;
using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Grid {
    public class Tile : MonoBehaviour {
        public int x;
        public int y;

        [Space] public LayerMask enemyLayer;

        private TileManager _tileManager;

        private void Start() {
            _tileManager = transform.root.GetComponent<TileManager>();
            _tileManager.tiles[x, y] = this;
        }

        public void Init(int xCord, int yCord){
            x = xCord;
            y = yCord;
        }

        public Vector3 ReturnTilePos(int x, int y) {
            return _tileManager.tiles[x, y].transform.position;
        }

        private void OnTriggerEnter2D(Collider2D col) {
            if (CheckLayerMask.IsInLayerMask(col.gameObject, enemyLayer)) {
                col.gameObject.GetComponent<EnemyBase>().tile = this;
            }
        }
    }
}
