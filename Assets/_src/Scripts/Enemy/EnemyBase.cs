using System;
using System.Collections;
using System.Globalization;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _src.Scripts.Enemy {
    public class EnemyBase : MonoBehaviour {
        public float hp; //BaseHp
        public float damage;

        [HideInInspector] public int x;
        [HideInInspector] public int y;
        
        [Header("Layers")]
        public LayerMask bulletLayer;

        [Header("UI Related")] public TextMeshProUGUI hpText;

        private float _currentHp; //Increment everytime at the y pos = 0, after 2 attack

        public void Init(int xCord, int yCord, float currentHp)
        {
            x = xCord;
            y = yCord;
            _currentHp = currentHp;
            hpText.text = $"{(int) currentHp}";
        }
        
        private void OnCollisionEnter2D(Collision2D col){
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer))
            {
                var damaged = col.gameObject.GetComponent<BulletBase>().damage;
                TakeDamage(damaged);
                hpText.text = $"{(int) _currentHp}";
            }
        }

        public void TakeDamage(float amount){
            _currentHp -= amount;

            if (_currentHp <= 0)
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
            this.SendMessage(EventType.EnemyDamagePlayer, damage);
            this.SendMessage(EventType.EnemyKilled, this);
            Destroy(gameObject);
        }
    }
}
