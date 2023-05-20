using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Managers;
using Scripts.Core.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.EnemyVariant {
    public enum SpecialOneMoves {
        Range,
        Melee
    }
    
    [Serializable]
    public struct EnemySpecialOneMoves {
        public SpecialOneMoves move;
        public float chance;
    }

    public class EnemySpecialOneAnim : EnemyAnim {
        public const string AttackMelee = "AttackMelee";
        public const string AttackRange = "AttackRange";
        public const string Deflect = "Deflect";
    }
    
    public class EnemySpecialOne : EnemyBase {
        [Header("Enemy Config")] 
        public List<EnemySpecialOneMoves> moves = new();
        public float deflectChance = 0.2f;

        private WeightedList<EnemySpecialOneMoves> _weightedMove = new();
        private SpecialOneMoves _currentMove; 

        public override void Init(int xCord, int yCord, float currHp) {
            base.Init(xCord, yCord, currHp);

            foreach (var set in moves) {
                _weightedMove.AddElement(set, set.chance);
            }
        }
        
        protected override void Move() {
            var randomAttack = _weightedMove.GetRandomItem();
            _currentMove = randomAttack.move;
            
            var emptyTiles = GridManager.GetEmptyTiles();
            if (emptyTiles.Count <= 0) {
                _currentMove = emptyTiles.Count > 0
                    ? _currentMove
                    : y <= 0 ? SpecialOneMoves.Melee : SpecialOneMoves.Range;
                
                Attack();
            }

            else {
                var tileToMoveTo = emptyTiles.Find(tile => {
                    var condition = _currentMove == SpecialOneMoves.Melee
                        ? tile.y == 0
                        : tile.y > 1;
                    
                    if (!condition) return false;
                    return tile.contains == Contains.None;
                });

                switch (tileToMoveTo) {
                    case null:
                        _currentMove = emptyTiles.Count > 0 
                            ? _currentMove 
                            : y <= 0 ? SpecialOneMoves.Melee : SpecialOneMoves.Range;
                        Attack();
                        break;
                    
                    default:
                        UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                        transform.DOMove(tileToMoveTo.transform.position, 1.4f).OnComplete(Attack);
                        break;
                }
            }
        }
        
        protected override void Attack() {
            _animator.SetTrigger(_currentMove == SpecialOneMoves.Melee
                ? EnemySpecialOneAnim.AttackMelee
                : EnemySpecialOneAnim.AttackRange);
        }

        public override void TakeDamage(float amount) {
            if (Random.value <= deflectChance && currentHp - amount > 0) {
                _animator.SetTrigger(EnemySpecialOneAnim.Deflect);
            }
            
            else {
                base.TakeDamage(amount);
            }
        }

        public override void OnFinishAttackAnimation() {
            hasFinishedTurn = true;
        }

        public override void OnAttackAnimationDamage() {
            var baseDamage = damage;

            var actualDamage = _currentMove switch {
                SpecialOneMoves.Melee => baseDamage * 1.4f,
                SpecialOneMoves.Range => baseDamage * 0.8f,
                _ => damage
            };

            Player.Player.instance.TakeDamage(actualDamage);
        }
    }
}