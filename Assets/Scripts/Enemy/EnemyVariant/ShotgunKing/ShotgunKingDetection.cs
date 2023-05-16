using Scripts.Core;
using UnityEngine;

namespace Scripts.Enemy.EnemyVariant.ShotgunKing {
    public class ShotgunKingDetection : MonoBehaviour {
        private EnemyShotgunKing _kingRef;
        public LayerMask bulletLayer;

        private void Start() {
            _kingRef = transform.parent.GetComponent<EnemyShotgunKing>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, bulletLayer)) return;
            _kingRef.Evade();
        }
    }
}
