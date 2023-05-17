using Enemy;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.Bullet {
    public enum BulletSpecialTag {
        None,
        Homing
    }
    
    public class BulletBase : MonoBehaviour {
        [Header("Config")]
        public float damage = 100f;
        public float speed = 3f;
        public int thresholdBounces = 12;

        [Header("Settings")] 
        public bool canBounce = true;
        public BulletSpecialTag specialTag;
        
        [Header("Layer Configs")]
        public LayerMask layersToInteract;

        [Space]
        public LayerMask enemyLayer;
        public LayerMask destroyLayer;

        private int _bouncedTimes;
        private bool _canRunBounceLogic = true;
        private bool _hasDestroyed;

        protected Vector3 Dir;
        protected Player.Player Player;
        protected Animator Animator;
        protected bool CanMove = true;

        protected void Start() {
            _bouncedTimes = 0;
            Player = global::Player.Player.instance;
            Animator = GetComponent<Animator>();
            
            OnSpawn();
        }

        protected virtual void OnSpawn() {
            Dir = transform.up;
        }

        protected virtual void FixedUpdate() {
            // This close to increasing physic render step for bounce logic
            // x-x
            // I LOVE CALCULATING REFLECTED VECTOR BY HAND AND HANDLING NINE THOUSANDS EDGE CASES
            /*
             * I set the raycast distance as high as possible because when you cast the ray, there's a chance that your
             * incoming angle is too shallow, therefore the raycast don't actually reach if you set it to a reasonable
             * number like 0.1f or some sort.
             */
            
            if (!CanMove) return;
            var result = Physics2D.Raycast(
                transform.position
                , Dir
                , 500f 
                , layersToInteract);

            if (result.collider == null) return;
            if (result.distance >= 0.5f) { //fine tune threshold let it be
                if (!_canRunBounceLogic) _canRunBounceLogic = true;
                transform.position += Dir * (speed * Time.fixedDeltaTime);
            }

            else {
                if (!_canRunBounceLogic) return;
                _canRunBounceLogic = false;
                
                if (canBounce) {
                    var reflected = Vector3.Reflect(Dir, result.normal);
                
                    Dir = reflected;
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, reflected);
                    
                    _bouncedTimes++;
                    if (_bouncedTimes >= thresholdBounces) {
                        OnBulletDestroy();
                    }
                }

                OnBounce(result.transform.gameObject);
            }
        }

        protected virtual void OnBounce(GameObject hitObject) {
            if (CheckLayerMask.IsInLayerMask(hitObject, enemyLayer)) {
                var enemyComp = hitObject.GetComponent<EnemyBase>();
                enemyComp.TakeDamage(damage);
            }
            
            else if (CheckLayerMask.IsInLayerMask(hitObject, destroyLayer)) {
                OnBulletDestroy();
            }
        }

        protected void OnBulletDestroy() {
            _hasDestroyed = true;
            EventDispatcher.instance.SendMessage(EventType.BulletDestroyed, gameObject);
            Destroy(gameObject);
        }

        private void OnBecameInvisible() {
            if (!_hasDestroyed) OnBulletDestroy();
        }
    }
}
