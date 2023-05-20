using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Scripts.Core.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.EnemyVariant.ShotgunKing {
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

        public override void Init(int xCord, int yCord, float currHp) {
            base.Init(xCord, yCord, currHp);
            
            foreach (var attack in attacks) {
                _attackWeighted.AddElement(attack.attackType, attack.chance);
            }
        }
        
        protected override void Move() {
            _currAttack = _attackWeighted.GetRandomItem();
            var emptyTiles = GridManager.GetEmptyTiles();

            switch (emptyTiles.Count <= 0) {
                case true:
                    _currAttack = y > 0 
                        ? _currAttack
                        : _attackWeighted.GetRandomItem(ShotgunKingAttackType.MeleeShotgunAndKnife);
                    break;

                default:
                    var tileToMoveTo = emptyTiles.Find(tile => {
                        var condition = _currAttack == ShotgunKingAttackType.MeleeShotgunAndKnife
                            ? tile.y == 0
                            : tile.y > 2;
                    
                        if (!condition) return false;
                        return tile.contains == Contains.None;
                    });

                    if (tileToMoveTo == null) {
                        _currAttack = y > 0
                            ? _attackWeighted.GetRandomItem(ShotgunKingAttackType.MeleeShotgunAndKnife)
                            : ShotgunKingAttackType.MeleeShotgunAndKnife;
                        Attack();
                    }

                    else {
                        UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
                        transform.DOMove(tileToMoveTo.transform.position, 2.2f).OnComplete(Attack);
                    }
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
            var emptyTiles = GridManager.GetEmptyTiles();
            
            if (emptyTiles.Count <= 0) return;
            var chance = Random.Range(0.0f, 1.01f);
            if (chance > evadeChance) return;

            StartCoroutine(Iframe(2.4f));
            var tileToMoveTo = emptyTiles.Find(tile => {
                if (tile.y <= 2) return false;
                if (Vector2.Distance(transform.position, tile.transform.position) <= 1f) return false;
                return tile.contains == Contains.None;
            });
            
            _animator.SetTrigger(ShotgunKingAnim.Evade);
            UpdatePosition(tileToMoveTo.x, tileToMoveTo.y);
            transform.DOMove(tileToMoveTo.transform.position, 0.1f);
        }

        protected override IEnumerator Iframe(float delay) {
            _col.enabled = false;
            _canTakeDamage = false;
            yield return new WaitForSeconds(delay);
            _col.enabled = true;
            _canTakeDamage = true;
        }
        
    }
}
