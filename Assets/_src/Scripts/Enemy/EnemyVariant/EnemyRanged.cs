using System.Collections;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy.EnemyWeapon;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Enemy.EnemyVariant
{
    /// <summary>
    /// Enemy thats shoot
    /// </summary>
    public class EnemyRanged : EnemyBase
    {
        /// <summary>
        /// Override of Attack, if reaches y = 0 attack and destroy player, else shoot at player every turn
        /// </summary>
        protected override void Attack() {
            switch (y) {
                //At y : 0
                case 0:
                    StartCoroutine(AttackAtY0Routine());
                    break;
                
                //At any Y
                default:
                    Shoot();
                    break;
            }
        }

        public override void OnFinishAttackAnimation() {
            hasFinishedTurn = true;
        }

        //Enemy Shoot Function
        private void Shoot() {
            _animator.SetTrigger(EnemyAnim.Attack);
        }

        private IEnumerator AttackAtY0Routine() {
            _animator.SetTrigger(EnemyAnim.Attack);
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            _animator.SetTrigger(EnemyAnim.Die);
            hasFinishedTurn = true;
        }
    }
}