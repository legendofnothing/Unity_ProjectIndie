using System;
using _src.Scripts.Core;
using DG.Tweening;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace _src.Scripts.Bullet {
    /// <summary>
    /// Base class for all Bullets, new BulletType will be derived from here
    /// </summary>
    public class BulletBase : MonoBehaviour {
        public float damage = 100f;
        public float speed = 3f;
        public int thresholdBounces = 12; //threshold to detect if the bullet keep bouncing left/right constantly

        [Space] 
        public LayerMask bounceLayer;
        public LayerMask destroyLayer;

        private int _bouncedTimes;
        private Vector3 _dir;

        protected void Start(){
            _bouncedTimes = 0;
            OnSpawn();
        }

        protected virtual void OnSpawn() {
            _dir = transform.up;                
        }

        private void FixedUpdate() {
            transform.position += _dir * (speed * Time.fixedDeltaTime);
        }

        protected virtual void OnCollisionEnter2D(Collision2D col) {
            if (CheckLayerMask.IsInLayerMask(col.gameObject, destroyLayer)) {
                Destroy(gameObject);
            }

            if (CheckLayerMask.IsInLayerMask(col.gameObject, bounceLayer)) {
                _dir = Vector3.Reflect(_dir, col.contacts[0].normal);
                transform.rotation = Quaternion.FromToRotation(Vector3.up, _dir);
                _bouncedTimes++;
                
                if (_bouncedTimes >= thresholdBounces) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
