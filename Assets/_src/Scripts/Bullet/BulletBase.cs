using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public class BulletBase : MonoBehaviour {
        public float damage;
        public float speed;

        [Space]
        public LayerMask destroyLayer;

        private Vector3 _lastVel;

        protected Rigidbody2D Rb;

        private void Start(){
            Rb = GetComponent<Rigidbody2D>();
            Rb.velocity = transform.up * speed;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col){
            if (CheckLayerMask.IsInLayerMask(col.gameObject, destroyLayer))
            {
                Destroy(gameObject);
            }
        }
    }
}
