using Bullet;
using UnityEngine;

namespace Scripts.Bullet.Types {
    public class BulletScatter : BulletBase {
        [Header("Bullet Config")] 
        [SerializeField] private GameObject scatterPiece;
        [Space] 
        [SerializeField] private int minScatter;
        [SerializeField] private int maxScatter;
        [Space]
        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle;
        
        protected override void OnBounce(GameObject hitObject) {
            var randomAmount = Random.Range(minScatter - 1, maxScatter + 1);
            for (var i = 0; i < randomAmount; i++) {
                var b = Instantiate(scatterPiece, transform.position, Quaternion.AngleAxis
                        (Random.Range(180 - minAngle, 180 + maxAngle), Vector3.forward) * transform.rotation);
                BulletManager.instance.AddBulletOnScene(b);
            }
            
            OnBulletDestroy();
        }
    }
}