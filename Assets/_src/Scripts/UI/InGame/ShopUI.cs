using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using EventDispatcher = _src.Scripts.Core.EventDispatcher.EventDispatcher;
using Image = UnityEngine.UI.Image;

namespace _src.Scripts.UI.InGame {
    public class ShopUI : MonoBehaviour {
        [Header("Refs")]
        public GameObject container;

        [Header("Misc")] 
        public GameObject aura;

        private Sequence _currAuraSequence;

        private void Start() {
            container.SetActive(false);
            EventDispatcher.instance.SubscribeListener(EventType.OpenShop, _=>OpenShop());
        }

        private void OpenShop() {
            container.SetActive(true);
            _currAuraSequence = DOTween.Sequence();
            _currAuraSequence
                .Append(aura.transform.DOScale(new Vector3(1.1f, 1.2f), 1.4f))
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnProceed() {
            _currAuraSequence.Kill();
            container.SetActive(false);
            EventDispatcher.instance.SendMessage(EventType.SwitchToPlayer);
            StartCoroutine(DelayInput());
        }

        private IEnumerator DelayInput() {
            Player.Player.instance.input.CanInput(false);
            yield return new WaitForSeconds(0.4f);
            Player.Player.instance.input.CanInput(true);
        }
    }
}
