using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _IsolatedTesting.BulletTest {
    public class BulletBounce : MonoBehaviour {
        public float speed;
        public LayerMask layers;

        [Space] 
        public Vector3 dir;

        [Space]
        public Vector3 prevPosition;
        
        // physic
        private bool canMove;

        private void Start() {
            dir = transform.up;
        }

        private void FixedUpdate() {
            prevPosition = transform.position;
            transform.position += dir * (speed * Time.fixedDeltaTime);
            
            var hits
                = Physics2D.RaycastAll(
                    transform.position,
                    (transform.position - prevPosition).normalized,
                    (transform.position - prevPosition).magnitude,
                    layers);
            
            if (hits.Length <= 0) return;
            
            var obj = hits.Length == 1
                ? hits[0]
                : hits.OrderBy(hit => (hit.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            
            var reflected = Vector3.Reflect(dir, obj.normal);
            var angleFromSurface = Vector3.Angle(reflected, obj.normal);
            if (angleFromSurface <= 10f) {
                var reflectedAngle = Mathf.Atan2( reflected.y , reflected.x );
                reflectedAngle += Random.Range(-60.0f, 60.0f) * Mathf.Deg2Rad;
                reflected = new Vector3((float)Math.Cos(reflectedAngle), (float) Math.Sin(reflectedAngle));
            } 
            
            
            dir = reflected;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, reflected.normalized);
        }
        

        private void OnDrawGizmos() { 
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.up ,transform.position + transform.up * (speed * Time.fixedDeltaTime));
        }
    }
}