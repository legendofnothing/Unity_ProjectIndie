using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _src.Scripts.Enemy
{
    public class FloatingCoin : MonoBehaviour
    {
        private int _amount;
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float duration;
    
        public void Init(int coinAdded)
        {
            _amount = coinAdded;
            text.text = $"+{_amount}";
            StartCoroutine(DestroyCoroutine());
        }

        IEnumerator DestroyCoroutine()
        {
            gameObject.transform.DOMoveY(transform.position.y + 0.6f, duration);

            yield return new WaitForSeconds(duration + 0.1f);
            Destroy(gameObject);
        }
    }
}
