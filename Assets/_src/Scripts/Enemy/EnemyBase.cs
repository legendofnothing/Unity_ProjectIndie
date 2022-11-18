using System.Collections;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _src.Scripts.Enemy {
    public class EnemyBase : MonoBehaviour {
        public float hp;
        public float damage;

        [HideInInspector] public int x;
        [HideInInspector] public int y;
        
        [Header("Layers")]
        public LayerMask bulletLayer;

        private int _timesAtY0; //Increment everytime at the y pos = 0, after 2 attack

        public void Init(int xCord, int yCord)
        {
            x = xCord;
            y = yCord;
            _timesAtY0 = 0;
        }
        
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
                this.SendMessage(EventType.EnemyKilled, this);
            }
        }
            
        public IEnumerator EnemyTurnCoroutine(Vector3 posToMove, int yCord)
        {
            Move(posToMove, yCord);

            yield return new WaitForSeconds(1.1f);
            
            Attack();
        }
        
        //P much move down
        protected virtual void Move(Vector3 posToMove, int yCord)
        {
            if (yCord < 0) return;

            var randomDuration = Random.Range(0.7f, 1f);
            transform.DOMove(posToMove, randomDuration);
            y--; //Update location
        }

        protected virtual void Attack()
        {
            if (y != 0) return;
            _timesAtY0++;
            UnityEngine.Debug.Log(_timesAtY0);

            if (_timesAtY0 == 2)
            {
                this.SendMessage(EventType.EnemyDamagePlayer, damage);
                Destroy(gameObject);
                this.SendMessage(EventType.EnemyKilled, this);
            }
        }
    }
}
