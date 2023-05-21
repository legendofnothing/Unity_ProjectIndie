using UnityEngine;

namespace _IsolatedTesting.BulletTest {
    public class PredictedPath : MonoBehaviour
    {
        public int maxReflectionCount = 5;
        public LayerMask layers;
        private void OnDrawGizmos() {

            DrawPredictedReflectionPattern(transform.position, transform.up, maxReflectionCount);
        }

        private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining) {
            while (true) {
                if (reflectionsRemaining <= 0) {
                    return;
                }

                var startingPosition = position;
                var hit = Physics2D.Raycast(position, transform.up, 300, layers);

                if (hit.collider != null) {
                    direction = Vector3.Reflect(direction, hit.normal);
                    position = hit.point;

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(startingPosition, position);
                }

                else {
                    position += direction * 300;

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(startingPosition, position);
                }


                reflectionsRemaining -= 1;
            }
        }
    }
}
