using System.Collections;
using DG.Tweening;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;
using SystemRandom = System.Random;

namespace Enemy.EnemyVariant {
    public class EnemyGhostAnim : EnemyAnim {
        public const string IsGhost = "isGhost";
        public const string GhostDisappear = "GhostDisappear";
        public const string GhostAppear = "GhostAppear";
        public const string GhostMove = "GhostMove";
    }
    
    public class EnemyGhost : EnemyBase {
        private bool _isInvisible; 

        protected override void Move() {
            var randomDuration = Random.Range(0.7f, 1f);
            
            if (!_isInvisible) {
                StartCoroutine(DisappearCoroutine(randomDuration));
            }

            else {
                _isInvisible = false;
                Change();
                Attack();
            }
        }

        public override void TakeDamage(float amount) {
            base.TakeDamage(amount);
            if (!_isInvisible) return;
            Change();
        }
        
        private IEnumerator DisappearCoroutine(float duration) {
            _animator.SetTrigger(EnemyGhostAnim.GhostDisappear);
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            _isInvisible = true;
            
            var emptyTiles = GridManager.GetEmptyTiles();
            var tileToMoveTo = emptyTiles[new SystemRandom().Next(emptyTiles.Count)];
            GridManager.SetTileContainContent(tileToMoveTo.x, tileToMoveTo.y, Contains.Enemy);

            transform   
                .DOMove(tileToMoveTo.transform.position, duration)
                .OnStart(() => {
                    _animator.SetTrigger(EnemyGhostAnim.GhostMove);
                    _animator.speed = duration / _animator.GetCurrentAnimatorClipInfo(0).Length;
                    _animator.SetBool(EnemyGhostAnim.IsGhost, true);
                })
                .OnComplete(() => {
                    GridManager.ResetTileContainContent(x, y);
                    UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                    _animator.speed = 1;
                    hasFinishedTurn = true;
                });
        }
        
        protected override void Attack() {
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine() {
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            _animator.SetTrigger(EnemyAnim.Attack);
        }
        
        public override void OnFinishAttackAnimation() {
            hasFinishedTurn = true;
        }

        private void Change() {
            _isInvisible = false;
            _animator.SetBool(EnemyGhostAnim.IsGhost, false);
            _animator.SetTrigger(EnemyGhostAnim.GhostAppear);
        }
    }
}