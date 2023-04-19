using System.Linq;
using _src.Scripts.Managers;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Bullet.Types {
    public class BulletHoming : BulletBase {
        [Header("Bullet Config")]
        [SerializeField] private Ease easeType
            ;
        protected override void OnSpawn() {
            var closestEnemy = EnemyManager.instance.enemies
                .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closestEnemy != null) 
                transform.DOMove(closestEnemy.transform.position, 1 / speed).SetEase(easeType);
        }
    }
}