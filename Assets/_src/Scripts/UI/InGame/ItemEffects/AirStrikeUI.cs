using System;
using System.Collections;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts.UI.InGame.ItemEffects {
    public class AirStrikeUI : MonoBehaviour {
        [Header("Config")]
        public float baseDamage;
        
        [Header("Refs")]
        public GameObject airStrikeGuide;
        public GameObject airStrikeGesture;
        [Space]
        public RectTransform aimingZone;
        public RectTransform plane;
        public RectTransform effectRect;
        [Space]
        public LayerMask enemyLayer;

        private Image _aimingZoneSprite;
        private Vector2 _aimingZoneSize;

        private Animator _effectAnimator;

        private RectTransform _parentCanvasRectTransform;
        private Canvas _parentCanvas;
        
        private bool _canInput;
        private bool _canAttack;

        private Sequence _currGestureSequence;

        public void PreInitAirStrikeUI() {
            _parentCanvas = transform.root.GetComponent<Canvas>();
            _parentCanvasRectTransform = _parentCanvas.GetComponent<RectTransform>();
            _aimingZoneSprite = aimingZone.gameObject.GetComponent<Image>();
            _effectAnimator = effectRect.GetComponent<Animator>();

            var v = new Vector3[4];
            aimingZone.GetWorldCorners(v);

            var h = Vector3.Distance(v[0], v[1]);
            var w = Vector3.Distance(v[1], v[2]);

            _aimingZoneSize = new Vector2(w, h);
        }

        public void InitAirStrikeUI() {
            airStrikeGuide.SetActive(true);
            aimingZone.gameObject.SetActive(false);
            _canInput = true;
            _canAttack = true;
            plane.anchoredPosition = new Vector2(0, 0);
            var col = _aimingZoneSprite.color;
            _aimingZoneSprite.color = new Color(col.r, col.g, col.b, 0.2f);
            _currGestureSequence = DOTween.Sequence();
            _currGestureSequence
                .Append(airStrikeGesture.transform.DOMoveX(-airStrikeGesture.transform.position.x, 2.8f))
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
        }
        
        private void Update() {
            if (Input.GetMouseButton(0) && _canInput) {
                if (airStrikeGuide.activeInHierarchy) airStrikeGuide.SetActive(false);
                if (!aimingZone.gameObject.activeInHierarchy) aimingZone.gameObject.SetActive(true);

                RectTransformUtility
                    .ScreenPointToLocalPointInRectangle(
                        _parentCanvasRectTransform
                        ,Input.mousePosition
                        ,Player.Player.instance.camera
                        ,out var vector );

                aimingZone.anchoredPosition = Vector3.Lerp(
                    aimingZone.anchoredPosition
                    , vector
                    , 0.4f);
            }

            if (Input.GetMouseButtonUp(0) && _canInput) {
                _canInput = false;
                var pos = Input.mousePosition;

                _aimingZoneSprite
                    .DOFade(0, 1.2f)
                    .OnComplete(() => {
                        plane.transform.position = new Vector3(
                            plane.transform.position.x
                            , aimingZone.transform.position.y);

                        RectTransformUtility
                            .ScreenPointToLocalPointInRectangle(
                                _parentCanvasRectTransform
                                ,pos
                                ,Player.Player.instance.camera
                                ,out var dest );
                        
                        //60 is rect width too lazy to create a ref
                        plane.transform
                            .DOLocalMoveX(-(Screen.width - 60f), 1.2f)
                            .SetEase(Ease.Linear)
                            .OnUpdate(() => {
                                if (plane.transform.localPosition.x < dest.x) {
                                    if (!_canAttack) return;
                                    _canAttack = false;

                                    var origin = Player.Player.instance.camera.ScreenToWorldPoint(pos);
                                    origin.z = 0;

                                    effectRect.anchoredPosition = dest;
                                    StartCoroutine(EffectRoutine());
                                    
                                    var hits = Physics2D.OverlapBoxAll(
                                        origin
                                        , _aimingZoneSize
                                        , enemyLayer);

                                    foreach (var hit in hits) {
                                        if (!hit.TryGetComponent(out EnemyBase enemy)) continue;
                                        var desiredDamage = baseDamage * SaveSystem.currentLevelData.TurnNumber;
                                        enemy.TakeDamage(desiredDamage);
                                    }
                                }
                            })
                            .OnComplete(() => {
                                EventDispatcher.instance.SendMessage(EventType.ReOpenUI);
                            });
                    });
            }
        }
        
        private IEnumerator EffectRoutine() {
            _effectAnimator.SetTrigger("Hit");
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => _effectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            effectRect.anchoredPosition = new Vector2(9999, 0);
        }
    }
}
