using System;
using System.Collections;
using System.Collections.Generic;
using _src.Scripts.Core.Collections;
using _src.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _src.Scripts.Enemy.EnemyVariant.ShotgunKing {
    public enum ShotgunKingAttackType {
        None = -1,
        RangeShotgunOnly = 0,
        RangeShotgunAndKnife = 1,
        MeleeShotgunAndKnife = 2
    }
    
    [Serializable]
    public struct ShotgunKingAttack {
        public ShotgunKingAttackType attackType;
        public float chance; 
    }
    
    public class ShotgunKingAnim : EnemyAnim {
        public const string Evade = "Evade";
    }

    public class EnemyShotgunKing : EnemyBase {
        [Header("Enemy Config")] 
        public float evadeChance;
        public List<ShotgunKingAttack> attacks = new();
        
        private WeightedList<ShotgunKingAttackType> _attackWeighted = new();
        private ShotgunKingAttackType _currAttack;
        private BoxCollider2D _col;

        public override void Init(int xCord, int yCord, float currHp) {
            base.Init(xCord, yCord, currHp);

            _col = gameObject.GetComponent<BoxCollider2D>();
            foreach (var attack in attacks) {
                _attackWeighted.AddElement(attack.attackType, attack.chance);
            }
        }
        
        protected override void Move() {
            var randomAttack = _attackWeighted.GetRandomItem();

            switch (randomAttack) {
                case ShotgunKingAttackType.MeleeShotgunAndKnife:
                    MoveBehavior(false, randomAttack);
                    break;
                default:
                    MoveBehavior(true, randomAttack);
                    break;
            }
        }
        
        protected override void Attack() {
            _animator.SetTrigger(_currAttack.ToString());
        }

        public override void OnFinishAttackAnimation() {
            _currAttack = ShotgunKingAttackType.None;
            hasFinishedTurn = true;
        }

        public void Evade() {
            var chance = Random.Range(0.0f, 1.01f);
            if (chance > evadeChance) return;

            StartCoroutine(EvadeRoutine());
            
            var emptyTiles = GridManager.GetEmptyTiles();
            var tileToMoveTo = emptyTiles.Find(tile => {
                if (tile.y <= 2) return false;
                if (Vector2.Distance(transform.position, tile.transform.position) <= 1f) return false;
                return tile.contains == Contains.None;
            });
            
            _animator.SetTrigger(ShotgunKingAnim.Evade);
            GridManager.SetTileContainContent(tileToMoveTo.x, tileToMoveTo.y, Contains.Enemy);
            transform.DOMove(tileToMoveTo.transform.position, 0.1f).OnComplete(() => {
                GridManager.ResetTileContainContent(x, y);
                UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
            });
        }

        private IEnumerator EvadeRoutine() {
            _col.enabled = false;
            _canTakeDamage = false;
            yield return new WaitForSeconds(2.4f);
            _col.enabled = true;
            _canTakeDamage = true;
        }

        #region Helper Functions

        private void MoveBehavior(bool isRanged, ShotgunKingAttackType type) {
            _currAttack = type;
            var emptyTiles = GridManager.GetEmptyTiles();
            Tile tileToMoveTo;
            
            if (isRanged) {
                tileToMoveTo = emptyTiles.Find(tile => {
                    if (tile.y <= 2) return false;
                    return tile.contains == Contains.None;
                });
            }

            else {
                tileToMoveTo = emptyTiles.Find(tile => {
                    if (tile.y != 0) return false;
                    return tile.contains == Contains.None;
                });
            }
            
            GridManager.SetTileContainContent(tileToMoveTo.x, tileToMoveTo.y, Contains.Enemy);
            transform.DOMove(tileToMoveTo.transform.position, 2.2f).OnComplete(() => {
                GridManager.ResetTileContainContent(x, y);
                UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                Attack();
            });
        }

        #endregion
    }
}
