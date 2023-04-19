using System.Collections;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Managers;
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

        private float _currentHp;
        protected GridManager GridManager;
        
        //State booleans 
        [HideInInspector] public bool hasFinishedTurn;
        
        /// <summary>
        /// Init function,call everytime a new enemy is instantiated 
        /// </summary>
        /// <param name="xCord">X Position on the Grid</param>
        /// <param name="yCord">Y Position on the Grid</param>
        /// <param name="currHp">Set Enemy HP w/ any modifiers</param>
        public void Init(int xCord, int yCord, float currHp) {
            x = xCord;
            y = yCord;
            _currentHp = currHp;
            hpText.text = $"{(int) _currentHp}";
            GridManager = GridManager.instance; 
        }
        
        public void OnEnemyTurn() {
            hasFinishedTurn = false;
            Move();
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col) {
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer)) {
                var damaged = col.gameObject.GetComponent<BulletBase>().damage;
                TakeDamage(damaged);
            }
        }

        public void TakeDamage(float amount) {
            _currentHp -= amount;
            hpText.text = $"{(int) _currentHp}";
            
            this.SendMessage(EventType.OnPlayerCoinAdd, coinAddedOnHit);
            this.SendMessage(EventType.AddScore, scoreAddedOnHit);
                
            SpawnFloatingCoin(coinAddedOnHit);
            
            //Execute Enemy if HP reaches 0
            if (!(_currentHp <= 0)) return;
            this.SendMessage(EventType.OnPlayerCoinAdd, coinAddedOnDestroy);
            this.SendMessage(EventType.AddScore, scoreAddedOnDestroy);
                
            SpawnFloatingCoin(coinAddedOnDestroy);
            this.SendMessage(EventType.EnemyKilled, this);
            Destroy(gameObject);
        }
        
        private void SpawnFloatingCoin(int amount) {
            var floatingCoin = Instantiate(floatingCoins, transform.position, Quaternion.identity);
            floatingCoin.GetComponent<FloatingCoin>().Init(amount);
        }
        
        protected virtual void Move() {
            if (y <= 0) return; 
            var updatedY = y - 1;
            GridManager.SetTileContainContent(x, updatedY,Contains.Enemy);
            var newPos = GridManager.tiles[x, updatedY].transform.position;
            var randomDuration = Random.Range(0.7f, 1f);
            
            transform.DOMove(newPos, randomDuration).OnComplete(() => {
                GridManager.ResetTileContainContent(x, y);
                UpdatePosition(x, updatedY);
                Attack();
            });
        }
        
        protected virtual void Attack() {
            if (y <= 0) {
                //Only attacks on Y = 0;
                Player.Player.instance.TakeDamage(damage);
                this.SendMessage(EventType.EnemyKilled, this);
                Destroy(gameObject);
            } 
            hasFinishedTurn = true;
        }

        protected void UpdatePosition(int newX, int newY) {
            x = newX;
            y = newY; 
        }
    }
}
