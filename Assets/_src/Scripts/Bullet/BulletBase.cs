using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public class BulletBase : MonoBehaviour {
        public float damage;
        public float speed;
        public int thresholdBounces; //threshold to detect if the bullet keep bouncing left/right constantly

        [Space] 
        public LayerMask boundLayer;
        public LayerMask destroyLayer;

        private int _bouncedTimes;

        protected Rigidbody2D Rb;

        private void Start(){
            Rb = GetComponent<Rigidbody2D>();
            Rb.velocity = transform.up * speed;

            _bouncedTimes = 0;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col){
            if (CheckLayerMask.IsInLayerMask(col.gameObject, destroyLayer))
            {
                Destroy(gameObject);
            }

            if (CheckLayerMask.IsInLayerMask(col.gameObject, boundLayer))
            {
                _bouncedTimes++;

                if (_bouncedTimes >= thresholdBounces)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
