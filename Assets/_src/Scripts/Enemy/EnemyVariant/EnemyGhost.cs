using System;
using _src.Scripts.Core;
using _src.Scripts.Enemy.EnemyWeapon;
using _src.Scripts.Managers;
using DG.Tweening;
using Random = UnityEngine.Random;
using SystemRandom = System.Random;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.Enemy.EnemyVariant {
    public class EnemyGhost : EnemyBase {
        [Header("Enemy Config")] 
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CanvasGroup enemyUI; 

        private bool _isInvisible; 

        protected override void Move() {
            var randomDuration = Random.Range(0.7f, 1f);
            
            if (!_isInvisible) {
                _isInvisible = true;
                var emptyTiles = GridManager.GetEmptyTiles();
                var tileToMoveTo = emptyTiles[new SystemRandom().Next(emptyTiles.Count)];
                GridManager.SetTileContainContent(
                    x
                    , y
                    , tileToMoveTo.x
                    , tileToMoveTo.y
                    , Contains.Enemy);
                spriteRenderer.DOFade(0, randomDuration);
                enemyUI.DOFade(0, randomDuration);
                transform
                    .DOMove(tileToMoveTo.transform.position, randomDuration)
                    .OnComplete(() => hasFinishedTurn = true);
            }

            else {
                _isInvisible = false;
                enemyUI.DOFade(1, randomDuration);
                spriteRenderer
                    .DOFade(255, randomDuration)
                    .OnComplete(Attack);
            }
        }
        
        protected override void Attack() {
            var bulletInst = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var bulletComp = bulletInst.GetComponent<EnemyBullet>();

            if (bulletComp == null) {   
                UnityEngine.Debug.Log($"Missing Bullet Script at {bulletInst}");
            }
            
            bulletComp.damage = damage;
            bulletInst.transform
                .DOMove(Player.Player.instance.transform.position, 0.4f)
                .OnComplete(() => hasFinishedTurn = true);
        }

        protected override void OnCollisionEnter2D(Collision2D col) {
            base.OnCollisionEnter2D(col);
            if (CheckLayerMask.IsInLayerMask(col.gameObject, bulletLayer)) {
                if (!_isInvisible) return;
                Change();
            }
        }

        private void Change() {
            var color = spriteRenderer.color;
            _isInvisible = false;
            spriteRenderer.color = new Color(color.r, color.g, color.b, 255);
            enemyUI.alpha = 1;
        }
    }
}