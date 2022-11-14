using System;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Grid;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Enemy {
    public class EnemyBase : MonoBehaviour {
        public float hp;
        
        [Header("Layers")]
        public LayerMask bulletLayer;

        public Tile tile;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                MoveDown();
            }
        }

        private void OnCollisionEnter2D(Collision2D col){
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer))
            {
                var damage = col.gameObject.GetComponent<BulletBase>().damage;
                TakeDamage(damage);
            }
        }

        public void TakeDamage(float amount){
            hp -= amount;

            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void MoveDown() {
            if (tile.y == 0) return;
            transform.DOMove(tile.ReturnTilePos(tile.x, tile.y - 1), 1);
        }
    }
}
