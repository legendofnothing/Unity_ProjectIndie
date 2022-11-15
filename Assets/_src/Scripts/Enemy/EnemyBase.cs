using System;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Grid;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Enemy {
    public class EnemyBase : MonoBehaviour {
        public float hp;

        [HideInInspector] public int x;
        [HideInInspector] public int y;
        
        [Header("Layers")]
        public LayerMask bulletLayer;

        public void Init(int xCord, int yCord)
        {
            x = xCord;
            y = yCord;
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
    }
}
