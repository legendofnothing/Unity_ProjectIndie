using UnityEngine;

namespace _src.Scripts.Bounds
{
    /// <summary>
    /// Creates a destroy line at the bottom of the screen
    /// </summary>
    public class DestroyBound : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private EdgeCollider2D _edgeCollider;
        
        private void Awake()
        {
            GenerateBounds();
        }

        public void GenerateBounds()
        {
            //Same calculation in Bounds.cs
            var w = 1 / (camera.WorldToViewportPoint(new Vector3(1, 1, 0)).x - .5f);
            var h = 1 / (camera.WorldToViewportPoint(new Vector3(1, 1, 0)).y - .5f);
                
            var pointA = new Vector2(w / 2, -h / 2);    //Bottom-Left
            var pointB = new Vector2(-w / 2, -h / 2);   //Bottom-Right

            var array = new[]
            {
                pointA, pointB
            };

            _edgeCollider.points = array;
        }
    }
}
