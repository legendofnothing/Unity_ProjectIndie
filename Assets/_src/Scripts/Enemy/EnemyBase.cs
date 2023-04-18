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
    
    /// <summary>
    /// Abstract class for different type of enemy to inherit from.
    /// Contains base behavior for enemies.
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour {
        public float hp = 100; //BaseHp
        public float damage = 10;
        
        [Space]
        public int coinAddedOnHit = 50;
        public int coinAddedOnDestroy = 100;

        [Space] 
        public int scoreAddedOnHit = 10;
        public int scoreAddedOnDestroy = 20;
        
        //Enemy Position on the grid
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
        
        /// <summary>
        /// Init function,call everytime a new enemy is instantiated 
        /// </summary>
        /// <param name="xCord">X Position on the Grid</param>
        /// <param name="yCord">Y Position on the Grid</param>
        /// <param name="currHp">Set Enemy HP w/ any modifiers</param>
        public void Init(int xCord, int yCord, float currHp)
        {
            x = xCord;
            y = yCord;
            currentHp = currHp;
            hpText.text = $"{(int) currentHp}";
            
            //Still very inefficient way to reference player. But only 1 Player instance per scene so it's fine.
            var player = FindObjectOfType<Player.Player>();
            if (player == null) UnityEngine.Debug.Log("$Missing player in scene, Script: {this}");
            playerTransform = player.transform;
        }
        
        /// <summary>
        /// Overridable in child' class
        /// </summary>
        /// <param name="col"></param>
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
        
        /// <summary>
        /// Take Damage Function
        /// </summary>
        /// <param name="amount">Damage Amount</param>
        public void TakeDamage(float amount){
            currentHp -= amount;
            
            //Execute Enemy if HP reaches 0
            if (currentHp <= 0)
            {
                Destroy(gameObject);
                this.SendMessage(EventType.EnemyKilled, this);
                
                this.SendMessage(EventType.OnPlayerCoinAdd, coinAddedOnDestroy);
                this.SendMessage(EventType.AddScore, scoreAddedOnDestroy);
                
                SpawnFloatingCoin(coinAddedOnDestroy);
            }
        }
        
        /// <summary>
        /// Spawn Floating Coin prefab
        /// </summary>
        /// <param name="amount">Amount of coin added to display</param>
        private void SpawnFloatingCoin(int amount)
        {
            var floatingCoin = Instantiate(floatingCoins, transform.position, Quaternion.identity);
            floatingCoin.GetComponent<FloatingCoin>().Init(amount);
        }
        
        /// <summary>
        /// Things to Execute on Enemy Turn, called in EnemyManager
        /// Move -> Shoot
        /// </summary>
        /// <param name="posToMove">Position to move to</param>
        /// <param name="yCord">New Y Cord</param>
        /// <returns></returns>
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
        
        //ATTACK!
        protected virtual void Attack()
        {
            if (y != 0) return; //Only attacks on Y = 0;
            Player.Player.instance.TakeDamage(damage);
            this.SendMessage(EventType.EnemyKilled, this);
            Destroy(gameObject);
        }
    }
}
