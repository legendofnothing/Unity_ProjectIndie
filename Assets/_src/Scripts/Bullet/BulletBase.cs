using System;
using _src.Scripts.Core;
using UnityEngine;

namespace Bullet {
    public class BulletBase : MonoBehaviour {
        public float damage;
        public float speed;

        [Space]
        public LayerMask destroyLayer;

        private Vector3 _lastVel;

        protected Rigidbody2D rigidbody2D;

        private void Start(){
            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.velocity = transform.up * speed;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col){
            if (CheckLayerMask.IsInLayerMask(col.gameObject, destroyLayer))
            {
                Destroy(gameObject);
            }
        }
    }
}
