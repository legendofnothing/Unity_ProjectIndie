using System;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using DG.Tweening;
using UnityEngine;

namespace _src.Scripts.Pickups.Bullets
{
    public class PickupBullet : PickupBase {
        public GameObject bullet;

        [Space] 
        public SpriteRenderer sprite1;
        public SpriteRenderer sprite2;
        
        private Sequence _currSequence;
        private bool _canPickup = true;

        private void Start() {
            _currSequence = DOTween.Sequence();
            _currSequence
                .Append(transform.DOMoveY(transform.position.y + 0.3f, 2.8f))
                .PrependInterval(0.1f)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnPickupTriggerEnter(Collider2D col) {
            if (!CheckLayerMask.IsInLayerMask(col.gameObject, pickupLayer)) return;
            if (!_canPickup) return;
            Player.Player.instance.bulletManager.AddBullet(bullet);
            _canPickup = false;
            Destroy();
        }

        public void Destroy() {
            _currSequence.Kill();
            _currSequence = DOTween.Sequence();
            _currSequence
                .Append(sprite1.DOFade(0, 1.2f))
                .Append(sprite2.DOFade(0, 1.2f))
                .Append(transform.DOMoveY(transform.position.y + 0.5f, 1.6f))
                .OnComplete(() => {
                    EventDispatcher.instance.SendMessage(EventType.PickupDestroyed, this);
                });
        }
    }
}