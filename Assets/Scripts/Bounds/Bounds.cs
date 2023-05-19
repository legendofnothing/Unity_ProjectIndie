using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Bounds {

    /// <summary>
    /// Create bounds that wraps around the Camera
    /// </summary>
    public class Bounds : MonoBehaviour { 
        [Serializable]
        public struct ColliderInfo {
            public ColliderPosition position;
            public BoxCollider2D collider;
        }
        public enum ColliderPosition {
            Top,
            Bottom,
            Right,
            Left
        }

        public List<ColliderInfo> colliders;
        public Camera boundCamera;

        private void Awake() {
            var cameraPos = boundCamera.transform.position;
            var screenSize = boundCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            foreach (var obj in colliders) {
                //the 1f is little offset so the thing overlaps to make sure no edge cases 
                obj.collider.transform.localScale = obj.position switch {
                    ColliderPosition.Top => new Vector3(screenSize.x * 2 + 1f, 1, 1),
                    ColliderPosition.Bottom => new Vector3(screenSize.x * 2 + 1f, 1, 1),
                    ColliderPosition.Right => new Vector3(1, screenSize.y * 2 + 1f, 1),
                    ColliderPosition.Left => new Vector3(1, screenSize.y * 2 + 1f, 1),
                    _ => throw new ArgumentOutOfRangeException()
                };

                obj.collider.transform.position = obj.position switch {
                    ColliderPosition.Top => new Vector3(
                            cameraPos.x
                        , screenSize.y + obj.collider.transform.localScale.y * 0.5f
                        , 0),
                    
                    ColliderPosition.Bottom => new Vector3(
                        cameraPos.x
                        , -screenSize.y - obj.collider.transform.localScale.y * 0.5f
                        , 0),
                    
                    ColliderPosition.Right => new Vector3(
                        screenSize.x + obj.collider.transform.localScale.x * 0.5f
                        , cameraPos.y
                        , 0),
                    
                    ColliderPosition.Left => new Vector3(
                         -screenSize.x - obj.collider.transform.localScale.x * 0.5f
                        , cameraPos.y
                        , 0),
                    
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}




