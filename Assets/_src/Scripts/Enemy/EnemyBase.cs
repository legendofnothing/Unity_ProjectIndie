using System.Collections;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;
using Vector3 = UnityEngine.Vector3;

namespace _src.Scripts.Enemy {
    public class EnemyAnim {
        public const string IsMoving = "isMoving";
        public const string Attack = "Attack";
        public const string Hit = "Hit";
        public const string Die = "Die";
    }
    
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
        public Slider healthBar;

        [Header("Floating Coins")] 
        public GameObject floatingCoins;

        private float _hp;
        private float _currentHp;
        [HideInInspector] public bool hasFinishedTurn;
        [HideInInspector] public bool isEnemyDying;
        protected GridManager GridManager;
        protected Animator _animator;

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
            _hp = currHp;
            GridManager = GridManager.instance;
            _animator = gameObject.GetComponent<Animator>();

            DOVirtual.Float(0, currHp, 1.2f, value => {
                hpText.text = $"{(int) value}";
            });
            healthBar.DOValue(currHp, 1.2f);
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
            if (_currentHp > 0) {
                Player.Player.instance.AddCoin(coinAddedOnHit);
                Player.Player.instance.AddScore(scoreAddedOnHit);
                
                hpText.text = $"{(int) _currentHp}";
                healthBar.DOValue(_currentHp / _hp, _currentHp / _hp);
                
                _animator.SetTrigger(EnemyAnim.Hit);
            }

            else {
                hpText.text = "0";
                healthBar.value = 0;
                
                Player.Player.instance.AddCoin(coinAddedOnDestroy);
                Player.Player.instance.AddScore(scoreAddedOnDestroy);
                
                _animator.SetTrigger(EnemyAnim.Die);
                isEnemyDying = true;
            }
        }

        public void OnFinishDeathAnimation() {
            GridManager.instance.SetTileContainContent(x, y, Contains.None);
            this.SendMessage(EventType.EnemyKilled, this);
            Destroy(gameObject);
        }

        public void OnAttackAnimationDamage() {
            Player.Player.instance.TakeDamage(damage);
        }

        public virtual void OnFinishAttackAnimation() {
            _animator.SetTrigger(EnemyAnim.Die);
            hasFinishedTurn = true;
        }

        private void SpawnFloatingCoin(int amount) {
            var floatingCoin = Instantiate(floatingCoins, transform.position, Quaternion.identity);
            floatingCoin.GetComponent<FloatingCoin>().Init(amount);
        }

        protected virtual void Move() {
            if (y <= 0) return; 
            var updatedY = y - 1;
            if (GridManager.tiles[x, updatedY].contains == Contains.Enemy) {
                hasFinishedTurn = true;
                return;
            }
            _animator.SetBool(EnemyAnim.IsMoving, true);
            GridManager.SetTileContainContent(x, updatedY,Contains.Enemy);
            var newPos = GridManager.tiles[x, updatedY].transform.position;
            var randomDuration = Random.Range(0.7f, 1f);
            
            transform.DOMove(newPos, randomDuration).OnComplete(() => {
                GridManager.ResetTileContainContent(x, y);
                UpdatePosition(x, updatedY);
                _animator.SetBool(EnemyAnim.IsMoving, false);
                Attack();
            });
        }
        
        protected virtual void Attack() {
            if (y <= 0) {
                _animator.SetTrigger(EnemyAnim.Attack);
            }

            else {
                hasFinishedTurn = true;
            }
        }

        protected void UpdatePosition(int newX, int newY) {
            x = newX;
            y = newY; 
        }
    }
}
