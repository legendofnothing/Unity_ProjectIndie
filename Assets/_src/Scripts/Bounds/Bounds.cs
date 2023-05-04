using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.Bounds {
    
    /// <summary>
    /// Create bounds that wraps around the Camera
    /// </summary>
    public class Bounds : MonoBehaviour {
        [SerializeField] private Camera camera;
        [SerializeField] private EdgeCollider2D edgeCollider;
        
        private void Awake(){
            GenerateBounds();
        }

        public void GenerateBounds(){
            //Weird calculation to convert screen pixels into world space height and width, the 0.5f is a must have offset
            var w = 1 / (camera.WorldToViewportPoint(new Vector3(1, 1, 0)).x - .5f);
            var h = 1 / (camera.WorldToViewportPoint(new Vector3(1, 1, 0)).y - .5f);

            var pointA = new Vector2(w / 2, h / 2);     //Top-left corner 
            var pointB = new Vector2(w / 2, -h / 2);    //Bottom-left corner
            var pointC = new Vector2(-w / 2, -h / 2);   //Bottom-right corner
            var pointD = new Vector2(-w / 2, h / 2);    //Top-right corner

            var array = new[] {
                pointC, pointD, pointA, pointB //From Bottom-Right -> Top-Right -> Top-Left -> Top-Right
            };

            //Assign edge collider points
            edgeCollider.points = array;
        }
    }
}
