using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy.EnemyWeapon;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Enemy.EnemyVariant
{
    public class EnemyRanged : EnemyBase
    {
        [Header("Enemy Config")] [SerializeField]
        private GameObject _bulletPrefab;

        /// <summary>
        /// Override of Attack, if reaches y = 0 attack and destroy player, else shoot at player every turn
        /// </summary>
        protected override void Attack()
        {
            switch (y)
            {
                case 0:
                    this.SendMessage(EventType.EnemyDamagePlayer, damage);
                    this.SendMessage(EventType.EnemyKilled, this);
                    Destroy(gameObject);
                    break;
                
                default:
                    Shoot();
                    break;
            }
        }

        private void Shoot()
        {
            var bulletInst = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            
            var bulletComp = bulletInst.GetComponent<EnemyBullet>();

            if (bulletComp == null)
            {
                UnityEngine.Debug.Log($"Missing Bullet Script at {bulletInst}");
            }
            
            bulletComp.damage = damage;
            bulletInst.transform.DOMove(playerTransform.position, 0.8f);
        }
    }
}