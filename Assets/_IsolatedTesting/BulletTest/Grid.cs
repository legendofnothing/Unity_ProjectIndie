using System;
using UnityEngine;

namespace _IsolatedTesting.BulletTest {
    public class Grid : MonoBehaviour {
        public int height;
        public int width;
        public GameObject tile;

        private void Start() {
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    var pos = transform.position
                              + new Vector3(
                                  x - (width - 1) / 2f,
                                  y - (height - 1) / 2f,
                                  0);
                    Instantiate(tile, pos, Quaternion.identity).transform.SetParent(gameObject.transform);
                }
            }
        }
    }
}
