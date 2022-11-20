using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public abstract class BulletBase : MonoBehaviour {
        public float damage = 100f;
        public float speed = 3f;
        private int _thresholdBounces = 12; //threshold to detect if the bullet keep bouncing left/right constantly

        private int _bouncedTimes;

        protected Rigidbody2D rb;

        private void Start(){
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = transform.up * speed;

            _bouncedTimes = 0;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col) {
            if (col.gameObject.layer == LayerMask.NameToLayer("DestroyBound"))
            {
                Destroy(gameObject);
            }

            if (col.gameObject.layer == LayerMask.NameToLayer("Bounds"))
            {
                _bouncedTimes++;

                if (_bouncedTimes >= _thresholdBounces)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
