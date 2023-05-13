using System;
using System.Linq;
using System.Security.Cryptography;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.Bullet {
    /// <summary>
    /// Base class for all Bullets, new BulletType will be derived from here
    /// </summary>
    public class BulletBase : MonoBehaviour {
        [Header("Config")]
        public float damage = 100f;
        public float speed = 3f;
        public int thresholdBounces = 12;

        [Header("Settings")] 
        public bool canBounce = true;
        
        [Header("Layer Configs")]
        public LayerMask layersToInteract;

        [Space]
        public LayerMask enemyLayer;
        public LayerMask destroyLayer;

        private int _bouncedTimes;
        private bool _canRunBounceLogic = true;

        protected Vector3 Dir;
        protected Player.Player Player;
        protected Animator Animator;
        protected bool CanMove = true;

        protected void Start() {
            _bouncedTimes = 0;
            Player = Scripts.Player.Player.instance;
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
                        OnDestroy();
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
                OnDestroy();
            }
        }

        protected void OnDestroy() {
            EventDispatcher.instance.SendMessage(EventType.BulletDestroyed, gameObject);
            Destroy(gameObject);
        }
    }
}
