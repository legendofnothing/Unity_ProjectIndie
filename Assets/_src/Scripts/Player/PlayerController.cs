using System;
using System.Collections;
using System.Collections.Generic;
using _src.Scripts.Bullet;
using UnityEngine;

namespace _src.Scripts.Player
{
    public class PlayerController : Player
    {
        public float maxAngle;
        
        public GameObject firingPoint;
        public GameObject aimingGuide;
        public GameObject bullet;

        [Header("Refs")] [SerializeField] private BulletManager _bulletManager;
        private Touch _touchInput;

        private bool _canInput = true;
        private enum TouchState
        {
            Dragging, 
            LetGo,
            None
        }

        private TouchState _touchState = TouchState.None;
        private SpriteRenderer _spriteRendererGuide;

        private void Start()
        {
            _spriteRendererGuide = aimingGuide.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_canInput) HandleInput();

            switch (_touchState)
            {
                case TouchState.Dragging:
                    RotatePlayer();
                    break;
                
                case TouchState.LetGo:
                    Shoot();
                    break;
                
                case TouchState.None:
                    Check();
                    break;
                
                default:
                    break;
            }
        }

        private void HandleInput()
        {
            switch (Input.touchCount)
            {
                case > 0:
                {
                    _touchInput = Input.GetTouch(0);
                    _touchState = _touchInput.phase switch
                    {
                        TouchPhase.Moved => TouchState.Dragging,
                        TouchPhase.Ended => TouchState.LetGo,
                        _ => _touchState
                    };

                    break;
                }
            }
        }

        #region TouchEvents

        private void RotatePlayer(){
            var touchPos = Camera.ScreenToWorldPoint(_touchInput.position);
            var position = transform.position;

            var angle = Mathf.Atan2(touchPos.y - position.y, touchPos.x - position.x) * Mathf.Rad2Deg - 90f;
            
            var angleRotateTo = 
                Quaternion.Euler(new Vector3(0, 0, ClampAngle(angle, -maxAngle, maxAngle)));
            
            transform.rotation = Quaternion.Slerp(transform.rotation, angleRotateTo, 0.2f);
                    
            _spriteRendererGuide.enabled = true;
        }
        
        private void Shoot(){
            _spriteRendererGuide.enabled = false;
            StartCoroutine(_bulletManager.SpawnBullet(firingPoint.transform.position, gameObject.transform.rotation));
            _touchState = TouchState.None;
        }

        private void Check(){
            _canInput = !_bulletManager.IsAllBulletsActive();
        }

        #endregion

        #region Clamp Angles
        /// <summary>
        /// Better clamping without switching value to maxValue like normal clamp
        /// when value is below minValue.
        /// </summary>
        /// <param name="angle">Angle to clamp</param>
        /// <param name="min">Minimum Range</param>
        /// <param name="max">Maximum Range</param>
        /// <returns></returns>
        private static float ClampAngle(float angle, float min, float max) {
            angle = NormalizeAngle(angle);
            switch (angle)
            {
                case > 180:
                    angle -= 360;
                    break;
                case < -180:
                    angle += 360;
                    break;
            }
 
            min = NormalizeAngle(min);
            switch (min)
            {
                case > 180:
                    min -= 360;
                    break;
                case < -180:
                    min += 360;
                    break;
            }
 
            max = NormalizeAngle(max);
            switch (max)
            {
                case > 180:
                    max -= 360;
                    break;
                case < -180:
                    max += 360;
                    break;
            }
            
            return Mathf.Clamp(angle, min, max);
        }
        
        /// <summary>
        /// Normalized angle value if its beyond (-360, 360)
        /// </summary>
        /// <param name="angle">Value to normalize</param>
        /// <returns></returns>
        private static float NormalizeAngle(float angle) {
            if (angle > 360) angle -= 360;
            if (angle < 0) angle += 360;
            return angle;
        }
        #endregion
    }
}
