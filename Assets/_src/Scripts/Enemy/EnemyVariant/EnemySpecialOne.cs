using System;
using System.Collections.Generic;
using _src.Scripts.Bullet;
using _src.Scripts.Core;
using _src.Scripts.Core.Collections;
using _src.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _src.Scripts.Enemy.EnemyVariant {
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

            switch (randomAttack.move) {
                case SpecialOneMoves.Melee:
                    var emptyTiles = GridManager.GetEmptyTiles();
                    var tileToMoveTo = emptyTiles.Find(tile => {
                        if (tile.y != 0) return false;
                        return tile.contains == Contains.None;
                    });
                    
                    GridManager.SetTileContainContent(tileToMoveTo.x, tileToMoveTo.y, Contains.Enemy);
                    transform.DOMove(tileToMoveTo.transform.position, 1.4f).OnComplete(() => {
                        GridManager.ResetTileContainContent(x, y);
                        UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                        Attack();
                    });
                    break;
                case SpecialOneMoves.Range:
                    var emptyTiles2 = GridManager.GetEmptyTiles();
                    var tileToMoveTo2 = emptyTiles2.Find(tile => {
                        if (tile.y <= 1) return false;
                        return tile.contains == Contains.None;
                    });
                    
                    GridManager.SetTileContainContent(tileToMoveTo2.x, tileToMoveTo2.y, Contains.Enemy);
                    transform.DOMove(tileToMoveTo2.transform.position, 1.4f).OnComplete(() => {
                        GridManager.ResetTileContainContent(x, y);
                        UpdatePosition(tileToMoveTo2.x, tileToMoveTo2.y);
                        Attack();
                    });
                    break;
            }
        }
        
        protected override void Attack() {
            _animator.SetTrigger(_currentMove == SpecialOneMoves.Melee
                ? EnemySpecialOneAnim.AttackMelee
                : EnemySpecialOneAnim.AttackRange);
        }

        protected override void OnCollisionEnter2D(Collision2D col) {
            if (!CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer)) return;
            var chance = Random.Range(0.0f, 1.0f);
            if (chance >= 0.4f) {
                base.OnCollisionEnter2D(col);
            }

            else {
                _animator.SetTrigger(EnemySpecialOneAnim.Deflect);
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