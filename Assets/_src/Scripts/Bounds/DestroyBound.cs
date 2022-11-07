using System;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using UnityEngine;

namespace _src.Scripts.Bounds
{
    public class DestroyBound : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private EdgeCollider2D _edgeCollider;
        [SerializeField] private BulletManager _bulletManager;
        public LayerMask bulletLayer; 

        private void Awake()
        {
            GenerateBounds();
        }

        public void GenerateBounds()
        {
            var w = 1 / (camera.WorldToViewportPoint(new Vector3(1, 1, 0)).x - .5f);
            var h = 1 / (camera.WorldToViewportPoint(new Vector3(1, 1, 0)).y - .5f);
                
            var pointA = new Vector2(w / 2, -h / 2);
            var pointB = new Vector2(-w / 2, -h / 2);

            var array = new[]
            {
                pointA, pointB
            };

            _edgeCollider.points = array;
        }
    }
}
