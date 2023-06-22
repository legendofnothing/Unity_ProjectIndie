using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Managers;
using ScriptableObjects;
using Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using AudioType = Managers.AudioType;
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
        public float hpModifier = 1; //BaseHp
        public float damageModifier = 1;
        [Space]
        public EnemySpawnType spawnType;
        public int movePriority;
        
        [Space]
        public float coinHitModifier = 1;
        public float coinDestroyModifier = 2;

        [Space] 
        public float scoreHitModifier = 1;
        public float scoreDestroyModifier = 2;
        
        //Enemy Position on the grid
        [HideInInspector] public int x;
        [HideInInspector] public int y;

        [Header("UI Related")]
        public Slider healthBar;

        private float _hp;
        protected float currentHp;
        protected float currentDmg;
        
        
        [HideInInspector] public bool hasFinishedTurn;
        [HideInInspector] public bool isEnemyDying;

        protected LevelData _levelData;
        protected BoxCollider2D _col;
        protected GridManager GridManager;
        protected Animator _animator;
        protected bool _canTakeDamage = true;
        
        public virtual void Init(int xCord, int yCord) {
            x = xCord;
            y = yCord;

            _levelData = LevelManager.instance.levelData;
            
            currentHp = _levelData.enemyBaseHP * hpModifier 
                        + _levelData.enemyBaseHP * hpModifier
                                                 * _levelData.enemyHPScale 
                                                 * (SaveSystem.currentLevelData.TurnNumber - 1);
            _hp = currentHp;
            currentDmg = _levelData.enemyBaseDMG * damageModifier;
            
            GridManager = GridManager.instance;
            _animator = gameObject.GetComponent<Animator>();
            _col = gameObject.GetComponent<BoxCollider2D>();

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
                _col.enabled = false;
                isEnemyDying = true;
                currentHp = -1;
                _canTakeDamage = false;
                
                EventDispatcher.instance.SendMessage(EventType.OnEnemyDying, this);
                healthBar.value = 0;
                
                Player.Player.instance.AddCoin(Mathf.FloorToInt(coinDestroyModifier * _levelData.coinAdd));
                Player.Player.instance.AddScore(Mathf.FloorToInt(scoreDestroyModifier * _levelData.scoreAdd));
                
                _animator.SetTrigger(EnemyAnim.Die);
                
                //Desperate check
                StartCoroutine(DesperateCheckIfEnemyDie());
            }

            else {
                currentHp -= amount;
                Player.Player.instance.AddCoin(Mathf.FloorToInt(coinHitModifier * _levelData.coinAdd));
                Player.Player.instance.AddScore(Mathf.FloorToInt(scoreHitModifier * _levelData.scoreAdd));
                
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
            Player.Player.instance.TakeDamage(currentDmg);
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

        public void PlayAudio(AudioType type) {
            AudioManagerHelper.instance.PlayEffect(type);
        }
    } 
}
