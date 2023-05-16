using System.Collections;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
using Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace Scripts.UI.InGame.ItemEffects {
    public class NuclearUI : MonoBehaviour {
        [Header("Ref")] 
        public RectTransform plane;
        public RectTransform bomb;
        public Image nuclearEffect;

        private Vector2 _originalPlanePos;
        private Vector2 _originalBombPos;
        private Vector3 _originalBombScale;
        private float _planeDestinationValue;
        public void PreInitNuclearUI() {
            _originalPlanePos = plane.anchoredPosition;
            _originalBombPos = bomb.anchoredPosition;
            _originalBombScale = bomb.transform.localScale;
            _planeDestinationValue = -(Screen.width + 60f);
        }
        
        public void InitNuclearUI() {
            plane.transform.DOLocalMoveX(_planeDestinationValue, 3.0f).SetEase(Ease.Linear);

            var seq = DOTween.Sequence();
            seq
                .Append(bomb.transform.DOLocalMoveX(-100f, 2.4f))
                .Join(bomb.transform.DOScale(new Vector3(0.1f, 0.1f, 1), 3.4f))
                .Append(nuclearEffect.transform.DOScale(new Vector3(1, 1, 1), 0.2f))
                .OnComplete(() => {
                    bomb.anchoredPosition = _originalBombPos;
                    plane.anchoredPosition = _originalPlanePos;
                    
                    foreach (var enemy in EnemyManager.instance.enemies) {
                        enemy.TakeDamage(99999f);
                    }

                    StartCoroutine(OnNukeDrop());
                });
            
        }

        private IEnumerator OnNukeDrop() { 
            yield return new WaitForSeconds(0.4f);
            var t = nuclearEffect.DOFade(0, 3.4f);
            yield return new DOTweenCYInstruction.WaitForCompletion(t);
            nuclearEffect.transform.localScale = new Vector3(0, 0, 1);
            bomb.transform.localScale = _originalBombScale;
            nuclearEffect.color = new Color(255, 255, 255, 1);
            EventDispatcher.instance.SendMessage(EventType.ReOpenUI);
        }
    }
}