using System;
using DG.Tweening;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;

namespace UI.InGame.Components {
    public class HeartBeatUIAnimation : MonoBehaviour {
        private enum State {
            Normal,
            Fast,
            Fastest
        }
        
        public GameObject heart;
        private Sequence _currSequence;
        private State _currState = State.Normal;

        private void Start() {
            EventDispatcher.instance
                .SubscribeListener(EventType.OnPlayerHPChange, hp => {
                    var value = (float)hp / Player.Player.instance.hp;

                    switch (value) {
                        case <= 0.5f and > 0.2f:
                            if (_currState != State.Fast) {
                                Sequence(State.Fast);
                                _currState = State.Fast;
                            }
                            break;
                        
                        case <= 0.2f:
                            if (_currState != State.Fastest) {
                                Sequence(State.Fastest);
                                _currState = State.Fastest;
                            }
                            break;
                        
                        default:
                            if (_currState != State.Normal) {
                                Sequence(State.Normal);
                                _currState = State.Normal;
                            }
                            break;
                    }
                });
            
            Sequence(State.Normal);
            _currState = State.Normal;
        }
        
        private void Sequence(State state) {
            _currSequence?.Pause();
            _currSequence = DOTween.Sequence();
            heart.transform.localScale = Vector3.one;
            
            switch (state) {
                case State.Fast:
                    _currSequence
                        .Append(heart.transform.DOScale(Vector3.one * 1.3f, 0.1f).SetLoops(4, LoopType.Yoyo))
                        .Append(heart.transform.DOScale(Vector3.one, 0.2f))
                        .AppendInterval(0.08f)
                        .SetLoops(-1, LoopType.Restart);
                    break;
                
                case State.Fastest:
                    _currSequence
                        .Append(heart.transform.DOScale(Vector3.one * 1.35f, 0.08f))
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                
                default:
                    _currSequence
                        .Append(heart.transform.DOScale(Vector3.one * 1.25f, 0.2f).SetLoops(4, LoopType.Yoyo))
                        .Append(heart.transform.DOScale(Vector3.one, 1.5f))
                        .AppendInterval(0.2f)
                        .SetLoops(-1, LoopType.Restart);
                    break;
            }
        }
    }
}
