using System;
using _src.Scripts.Core;
using Bullet;
using UnityEngine;

public class EnemyBase : MonoBehaviour {
    public float hp;
    public LayerMask bulletLayer;

    private void OnCollisionEnter2D(Collision2D col){
        if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer))
        {
            var damage = col.gameObject.GetComponent<BulletBase>().damage;
            TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount){
        hp -= amount;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
