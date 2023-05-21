using System;
using System.Linq;
using Enemy;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = Scripts.Core.EventDispatcher.EventType;
using Random = UnityEngine.Random;

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
            if (!CanMove) return;
            
            var prevPosition = transform.position;
            transform.position += Dir * (speed * Time.fixedDeltaTime);
            
            var hit
                = Physics2D.Raycast(
                    transform.position,
                    (transform.position - prevPosition).normalized,
                    (transform.position - prevPosition).magnitude,
                    layersToInteract);

            if (hit.collider == null) return;

            if (canBounce) {
                var reflected = Vector3.Reflect(Dir, hit.normal);
                var angleFromSurface = Vector3.Angle(reflected, hit.normal);
                if (angleFromSurface <= 10f) {
                    var reflectedAngle = Mathf.Atan2( reflected.y , reflected.x );
                    reflectedAngle += Random.Range(-10.0f, 15.0f) * Mathf.Deg2Rad;
                    reflected = new Vector3((float)Math.Cos(reflectedAngle), (float) Math.Sin(reflectedAngle));
                }

                Dir = reflected.normalized;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, reflected.normalized);

                _bouncedTimes++;
                if (_bouncedTimes >= thresholdBounces) {
                    OnBulletDestroy();
                }
            }
            OnBounce(hit.transform.gameObject);
        }

        protected virtual void OnBounce(GameObject hitObject) {
            if (CheckLayerMask.IsInLayerMask(hitObject, enemyLayer)) {
                var enemyComp = hitObject.GetComponent<EnemyBase>();
                if (enemyComp.isEnemyDying) return;
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
