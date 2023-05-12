using System;
using System.Linq;
using System.Security.Cryptography;
using _src.Scripts.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = System.Numerics.Vector2;

namespace _src.Scripts.Bullet {
    /// <summary>
    /// Base class for all Bullets, new BulletType will be derived from here
    /// </summary>
    public class BulletBase : MonoBehaviour {
        public float damage = 100f;
        public float speed = 3f;
        public int thresholdBounces = 12; //threshold to detect if the bullet keep bouncing left/right constantly

        [FormerlySerializedAs("canBounce")] [Space] 
        public bool notBounce;
        public bool notDestroyAtBottomOfScreen;
        public LayerMask bounceLayer;

        private int _bouncedTimes;
        private Vector3 _dir;
        protected Player.Player _player;

        protected void Start() {
            _bouncedTimes = 0;
            _player = Player.Player.instance;
            OnSpawn();
        }

        protected virtual void OnSpawn() {
            _dir = transform.up;
        }

        private void FixedUpdate() {
            ScreenBounce();
            transform.position += _dir * (speed * Time.fixedDeltaTime);
        }

        private void OnBecameInvisible() {
            Destroy(gameObject);
        }

        private void ScreenBounce() {
            var projectedPos = transform.position + _dir * (speed * Time.fixedDeltaTime);
            
            var atLeftEdge   = projectedPos.x < _player.screenFloats.LeftScreen;
            var atRightEdge  = projectedPos.x > _player.screenFloats.RightScreen;
            var atTopEdge    = projectedPos.y > _player.screenFloats.TopScreen;
            var atBottomEdge = projectedPos.y < _player.screenFloats.BottomScreen + 0.2f;

            if (atLeftEdge || atRightEdge || atTopEdge) {
                if (notBounce) return;
                
                var vel = _dir * speed;
                var normal = new Vector3(atLeftEdge ? 1 : atRightEdge ? -1 : 0,
                    atTopEdge ? -1 : atBottomEdge ? 1 : 0);
                var reflected = Vector3.Reflect(vel, normal);

                var angle = Vector3.Angle(reflected, _dir);
                if (angle >= 25f) {
                    _dir = Quaternion.FromToRotation(_dir, reflected.normalized) * _dir;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, _dir);
                }
                
                _bouncedTimes++;
                if (_bouncedTimes >= thresholdBounces) {
                    Destroy(gameObject);
                }
            }

            else if (atBottomEdge) {
                if (!notDestroyAtBottomOfScreen) Destroy(gameObject);
                else {
                    var vel = _dir * speed;
                    var normal = new Vector3(atLeftEdge ? 1 : atRightEdge ? -1 : 0,
                        atTopEdge ? -1 : atBottomEdge ? 1 : 0);
                    var reflected = Vector3.Reflect(vel, normal);

                    var angle = Vector3.Angle(reflected, _dir);
                    if (angle >= 10f) {
                        _dir = Quaternion.FromToRotation(_dir, reflected.normalized) * _dir;
                        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dir);
                    }
                
                    _bouncedTimes++;
                    if (_bouncedTimes >= thresholdBounces) {
                        Destroy(gameObject);
                    }
                }
            }   
        }

        protected virtual void OnCollisionEnter2D(Collision2D other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, bounceLayer)) return;
            var reflected = Vector3.Reflect(_dir, other.contacts[0].normal);
            _dir = reflected;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, _dir);
            OnBounce();
        }

        protected virtual void OnBounce() {
            _bouncedTimes++;
            if (_bouncedTimes >= thresholdBounces) {
                Destroy(gameObject);
            }
        }
    }
}
