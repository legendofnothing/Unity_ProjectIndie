using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using Managers;
using Random = UnityEngine.Random;
using SystemRandom = System.Random;

namespace Enemy.EnemyVariant {
    public class EnemyJumper : EnemyBase {
        protected override void Move() {
            var emptyTiles = GridManager.GetEmptyTiles();

            switch (emptyTiles.Count <= 0) {
                case true:
                    Attack();
                    break;
                
                default:
                    var rnd = new SystemRandom();
                    var tileToMoveTo = 
                        emptyTiles
                            .OrderBy(_=>rnd.Next())
                            .FirstOrDefault(tile => tile.y > 0 && tile.y < GridManager.height - 1 && tile.contains == Contains.None);
                    
                    if (tileToMoveTo == null) Attack();
                    else {
                        var randomDuration = Random.Range(0.7f, 1f);
                        UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                    
                        transform.DOMove(tileToMoveTo.transform.position, randomDuration)
                            .OnStart(() => { 
                                _animator.speed = randomDuration / _animator.GetCurrentAnimatorClipInfo(0).Length;
                                _animator.SetBool(EnemyAnim.IsMoving, true);
                            })
                            .OnComplete(() => {
                                _animator.speed = 1;
                                _animator.SetBool(EnemyAnim.IsMoving, false);
                                Attack();
                            });   
                    }
                    break;
            }
        }
        
        protected override void Attack() {
            _animator.SetTrigger(EnemyAnim.Attack);
        }
        
        public override void OnFinishAttackAnimation() {
            hasFinishedTurn = true;
        }
    }
}