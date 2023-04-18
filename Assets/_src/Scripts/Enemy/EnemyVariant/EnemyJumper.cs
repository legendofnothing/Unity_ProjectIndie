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
        [FormerlySerializedAs("_bulletPrefab")]
        [Header("Enemy Config")] 
        [SerializeField] private GameObject bulletPrefab;
        protected override void Move() {
            var emptyTiles = GridManager.GetEmptyTiles();
            var tileToMoveTo = emptyTiles[new SystemRandom().Next(emptyTiles.Count)];
            
            var randomDuration = Random.Range(0.7f, 1f);
            GridManager.SetTileContainContent(
                x
                , y
                , tileToMoveTo.x
                , tileToMoveTo.y
                , Contains.Enemy);
            transform.DOMove(tileToMoveTo.transform.position, randomDuration).OnComplete(Attack);
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
    }
}