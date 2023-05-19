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
        public float bulletRadius = 0.4f;

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
        private RaycastHit2D[] _hits;

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
            transform.position += Dir * (speed * Time.fixedDeltaTime);
            _hits = Physics2D.CircleCastAll(
                transform.position,
                bulletRadius,
                Vector2.zero,
                0,
                layersToInteract
            );

            if (_hits.Length <= 0) return;
            var obj = _hits.Length == 1
                ? _hits[0]
                : _hits.OrderBy(hit => (hit.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            
            if (canBounce) {
                var reflected = Vector3.Reflect(Dir, obj.normal);
                var angleFromSurface = Vector3.Angle(reflected, obj.normal);
                if (angleFromSurface <= 10f) {
                    var reflectedAngle = Mathf.Atan2( reflected.y , reflected.x );
                    reflectedAngle += Random.Range(-10.0f, 15.0f) * Mathf.Deg2Rad;
                    reflected = new Vector3((float)Math.Cos(reflectedAngle), (float) Math.Sin(reflectedAngle));
                }

                Dir = reflected;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, reflected);

                _bouncedTimes++;
                if (_bouncedTimes >= thresholdBounces) {
                    OnBulletDestroy();
                }

            }

            foreach (var hit in _hits) {
                OnBounce(hit.transform.gameObject);
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
