using System.Collections;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _src.Scripts.Enemy {
    public abstract class EnemyBase : MonoBehaviour {
        public float hp = 100; //BaseHp
        public float damage = 10;
        
        [Space]
        public int coinAddedOnHit = 50;
        public int coinAddedOnDestroy = 100;

        [Space] 
        public int scoreAddedOnHit = 10;
        public int scoreAddedOnDestroy = 20;
        
        [HideInInspector] public int x;
        [HideInInspector] public int y;
        
        [Header("Layers")]
        public LayerMask bulletLayer;

        [Header("UI Related")] 
        public TextMeshProUGUI hpText;

        [Header("Floating Coins")] 
        public GameObject floatingCoins;

        protected float currentHp; //Increment everytime at the y pos = 0, after 2 attack
        protected Transform playerTransform; 

        public void Init(int xCord, int yCord, float currHp)
        {
            x = xCord;
            y = yCord;
            currentHp = currHp;
            hpText.text = $"{(int) currentHp}";

            var player = FindObjectOfType<Player.Player>();
            if (player == null) UnityEngine.Debug.Log("$Missing player in scene, Script: {this}");
            playerTransform = player.transform;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col){
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer))
            {
                var damaged = col.gameObject.GetComponent<BulletBase>().damage;
                TakeDamage(damaged);
                hpText.text = $"{(int) currentHp}";

                if (currentHp <= 0) return;
                
                this.SendMessage(EventType.OnPlayerCoinAdd, coinAddedOnHit);
                this.SendMessage(EventType.AddScore, scoreAddedOnHit);
                
                SpawnFloatingCoin(coinAddedOnHit);
            }
        }

        public void TakeDamage(float amount){
            currentHp -= amount;

            if (currentHp <= 0)
            {
                Destroy(gameObject);
                this.SendMessage(EventType.EnemyKilled, this);
                
                this.SendMessage(EventType.OnPlayerCoinAdd, coinAddedOnDestroy);
                this.SendMessage(EventType.AddScore, scoreAddedOnDestroy);
                
                SpawnFloatingCoin(coinAddedOnDestroy);
            }
        }

        public void SpawnFloatingCoin(int amount)
        {
            var floatingCoin = Instantiate(floatingCoins, transform.position, Quaternion.identity);
            floatingCoin.GetComponent<FloatingCoin>().Init(amount);
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
