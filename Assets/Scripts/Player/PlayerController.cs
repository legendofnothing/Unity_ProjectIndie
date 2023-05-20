using System.Collections.Generic;
using Scripts.Bullet;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using UnityEngine.EventSystems;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Player
{
    /// <summary>
    /// Handles Player Controller
    /// </summary>
    public class PlayerController : MonoBehaviour {
        [Space] 
        public float safePixelsWidth;
        public float minHoldDuration;
        
        private float _startTime;
        private bool _canSetTime;

        [Space] 
        public LayerMask uiLayer;
        
        [Space]
        public float maxAngle;
        
        [Space]
        public GameObject firingPoint;

        [Header("Refs")] 
        [SerializeField] private BulletManager _bulletManager;
        
        private bool _canInput;

        private enum TouchState {
            Aiming, 
            Shooting,
            Default
        }

        private TouchState _touchState = TouchState.Default;
        private SpriteRenderer[] _spriteRendererGuide;

        private void Start() {
            _canInput = true;
        }

        private void Update() {
            if (_canInput) HandleInput();

            switch (_touchState) {
                case TouchState.Aiming:
                    RotatePlayer();
                    break;
                
                case TouchState.Shooting:
                    Shoot();
                    break;
                
                case TouchState.Default:
                    break;
                
                default:
                    break;
            }
        }
        
        private void HandleInput() {
            if (Input.GetMouseButton(0)) {
                if (_touchState == TouchState.Default) {
                    _touchState = TouchState.Aiming;
                    
                    if (_canSetTime) {
                        _canSetTime = false;
                        _startTime = Time.time;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (Mathf.Abs(Input.mousePosition.x) >= Screen.width - safePixelsWidth) return;
                if (IsOverUI()) return;
                if ((Time.time - _startTime) <= minHoldDuration) {
                    _canSetTime = true;
                    return;
                }
                _canSetTime = true;
                _touchState = TouchState.Shooting;
            }
        }

        #region TouchEvents

        private void RotatePlayer(){
            var touchPos = Player.instance.playerCamera.ScreenToWorldPoint(Input.mousePosition);
            var position = transform.position;

            var angle = Mathf.Atan2(touchPos.y - position.y, touchPos.x - position.x) * Mathf.Rad2Deg - 90f;
            
            var angleRotateTo = 
                Quaternion.Euler(new Vector3(0, 0, ClampAngle(angle, -maxAngle, maxAngle)));
            
            transform.rotation = Quaternion.Slerp(transform.rotation, angleRotateTo, 10f * Time.fixedDeltaTime);
        }
        
        private void Shoot(){
            _touchState = TouchState.Default;
            EventDispatcher.instance.SendMessage(EventType.SwitchToShooting);
            StartCoroutine(_bulletManager.SpawnBullet(firingPoint.transform.position, gameObject.transform.rotation));
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

        #region Helper Functions
        public void CanInput(bool condition) => _canInput = condition;

        private bool IsOverUI() {
            //raycast from mouse pos to all UI elements 
            var results = new List<RaycastResult>();
            var eventData = new PointerEventData(EventSystem.current) {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(eventData, results);
            var hit = results.Find(result => CheckLayerMask.IsInLayerMask(result.gameObject, uiLayer));
            return hit.gameObject != null;
        }
        #endregion
    }
}
