using System;
using System.Security.Cryptography;
using UnityEngine;

namespace Bullet {
    public class BulletBase : MonoBehaviour {
        public float damage;
        public float speed;

        private Vector3 _lastVel;
        
        protected Rigidbody2D rigidbody2D;
        
        private void Start(){
            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.velocity = transform.up * speed;
        }

        private void Update(){
            _lastVel = rigidbody2D.velocity;
        }

        protected virtual void OnCollisionEnter2D(Collision2D col){
            if (col)
        }
    }
}
