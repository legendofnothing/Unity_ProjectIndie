using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using EventDispatcher = Scripts.Core.EventDispatcher.EventDispatcher;
using EventType = Scripts.Core.EventDispatcher.EventType;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

namespace Enemy {
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
        public EnemySpawnType spawnType;
        public int movePriority;
        
        [Space]
        public int coinAddedOnHit = 50;
        public int coinAddedOnDestroy = 100;

        [Space] 
        public int scoreAddedOnHit = 10;
        public int scoreAddedOnDestroy = 20;
        
        //Enemy Position on the grid
        [HideInInspector] public int x;
        [HideInInspector] public int y;
        [HideInInspector] public Tile currentDestination;

        [Header("UI Related")] 
        public TextMeshProUGUI hpText;
        public Slider healthBar;

        private float _hp;
        protected float currentHp;
        [HideInInspector] public bool hasFinishedTurn;
        [HideInInspector] public bool isEnemyDying;
        protected BoxCollider2D _col;
        protected GridManager GridManager;
        protected Animator _animator;
        protected bool _canTakeDamage = true;
        
        public virtual void Init(int xCord, int yCord, float currHp) {
            x = xCord;
            y = yCord;
            currentHp = currHp;
            _hp = currHp;
            GridManager = GridManager.instance;
            _animator = gameObject.GetComponent<Animator>();
            _col = gameObject.GetComponent<BoxCollider2D>();

            hpText.text = currHp.ToString("0.0");
            healthBar.value = 1;
        }
        
        public void OnEnemyTurn() {
            hasFinishedTurn = false;
            Move();
        }

        public virtual void TakeDamage(float amount) {
            if (!_canTakeDamage) return;
            StartCoroutine(Iframe(0.1f));

            if (currentHp - amount < 0) {
                isEnemyDying = true;
                currentHp = -1;
                _canTakeDamage = false;
                _col.enabled = false;
                
                EventDispatcher.instance.SendMessage(EventType.OnEnemyDying, this);
                hpText.text = "0.0";
                healthBar.value = 0;
                
                Player.Player.instance.AddCoin(coinAddedOnDestroy);
                Player.Player.instance.AddScore(scoreAddedOnDestroy);
                
                _animator.SetTrigger(EnemyAnim.Die);
                
                //Desperate check
                StartCoroutine(DesperateCheckIfEnemyDie());
            }

            else {
                currentHp -= amount;
                Player.Player.instance.AddCoin(coinAddedOnHit);
                Player.Player.instance.AddScore(scoreAddedOnHit);

                hpText.text = currentHp.ToString("0.0");
                healthBar.value = currentHp / _hp;
                
                _animator.SetTrigger(EnemyAnim.Hit);
            }
        }

        private IEnumerator DesperateCheckIfEnemyDie() {
            var clip = _animator.runtimeAnimatorController.animationClips.FirstOrDefault(c => c.name == EnemyAnim.Die);
            if (clip != null) yield return new WaitForSeconds(clip.length + 0.8f);
            if (gameObject.activeInHierarchy) {
                _animator.SetTrigger(EnemyAnim.Die);
            }
        }

        public void OnFinishDeathAnimation() {
            GridManager.instance.SetTileContainContent(x, y, Contains.None);
            EventDispatcher.instance.SendMessage(EventType.EnemyKilled, this);
            Destroy(gameObject);
        }

        public virtual void OnAttackAnimationDamage() {
            Player.Player.instance.TakeDamage(damage);
        }

        public virtual void OnFinishAttackAnimation() {
            _animator.SetTrigger(EnemyAnim.Die);
            hasFinishedTurn = true;
        }
        
        protected virtual void Move() {
            if (y <= 0) return; 
            var updatedY = y - 1;
            
            if (GridManager.tiles[x, updatedY].contains == Contains.Enemy) Attack();
            else {
                UpdatePosition(x, updatedY);
                _animator.SetBool(EnemyAnim.IsMoving, true);
            
                var newPos = GridManager.tiles[x, updatedY].transform.position;
                var randomDuration = Random.Range(0.7f, 1f);
            
                transform.DOMove(newPos, randomDuration).OnComplete(() => {
                    _animator.SetBool(EnemyAnim.IsMoving, false);
                    Attack();
                });
            }
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
            GridManager.SetTileContainContent(newX, newY,Contains.Enemy);
            GridManager.ResetTileContainContent(x, y);
            x = newX;
            y = newY;
        }

        protected virtual IEnumerator Iframe(float delay) {
            _canTakeDamage = false;
            yield return new WaitForSeconds(delay);
            _canTakeDamage = true;
        }
    }
}
