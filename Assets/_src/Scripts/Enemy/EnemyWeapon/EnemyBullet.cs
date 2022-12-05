using System.Collections;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Enemy.EnemyWeapon
{
    /// <summary>
    /// Enemy Bullet Behavior
    /// </summary>
    public class EnemyBullet : MonoBehaviour
    {
        [HideInInspector] public float damage;
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            StartCoroutine(DestroyBullet());
        }

        private IEnumerator DestroyBullet()
        {
            yield return new WaitForSeconds(0.1f);
            
            this.SendMessage(EventType.EnemyDamagePlayer, damage);

            transform.DOKill(this);
            Destroy(gameObject);
        }
    }
}
