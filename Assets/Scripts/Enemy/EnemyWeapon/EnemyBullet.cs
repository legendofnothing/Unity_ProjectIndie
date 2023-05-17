using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Enemy.EnemyWeapon
{
    /// <summary>
    /// Enemy Bullet Behavior
    /// </summary>
    public class EnemyBullet : MonoBehaviour
    {
        [HideInInspector] public float damage;
        
        private void OnCollisionEnter2D(Collision2D col) {
            StartCoroutine(DestroyBullet());
        }

        private IEnumerator DestroyBullet() {
            yield return new WaitForSeconds(0.1f);
            Player.Player.instance.TakeDamage(damage);
            transform.DOKill(this);
            Destroy(gameObject);
        }
    }
}
