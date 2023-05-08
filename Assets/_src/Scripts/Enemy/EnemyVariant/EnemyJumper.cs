using System.Linq;
using _src.Scripts.Enemy.EnemyWeapon;
using _src.Scripts.Managers;
using DG.Tweening;
using Random = UnityEngine.Random;
using SystemRandom = System.Random;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.Enemy.EnemyVariant {
    public class EnemyJumper : EnemyBase {
        [Header("Enemy Config")] 
        [SerializeField] private GameObject bulletPrefab;
        protected override void Move() {
            var emptyTiles = GridManager.GetEmptyTiles();
            var tileToMoveTo = emptyTiles[new SystemRandom().Next(emptyTiles.Count)];
            
            var randomDuration = Random.Range(0.7f, 1f);
            
            GridManager.SetTileContainContent(x, y, Contains.Enemy);
            transform.DOMove(tileToMoveTo.transform.position, randomDuration).OnStart(() => { 
                _animator.speed = randomDuration / _animator.GetCurrentAnimatorClipInfo(0).Length;
                _animator.SetBool(EnemyAnim.IsMoving, true);
            })
            .OnComplete(() => {
                GridManager.ResetTileContainContent(x, y);
                UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                _animator.speed = 1;
                _animator.SetBool(EnemyAnim.IsMoving, false);
                Attack();
            });
        }
        
        protected override void Attack() {
            _animator.SetTrigger(EnemyAnim.Attack);
        }
        
        public override void OnFinishAttackAnimation() {
            hasFinishedTurn = true;
        }
    }
}