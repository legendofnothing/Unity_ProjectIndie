using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Scripts.Enemy
{
    /// <summary>
    /// Spawns a floating text displaying how much coin the player earns 
    /// </summary>
    public class FloatingCoin : MonoBehaviour
    {
        private int _amount;
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float duration;
        
        /// <summary>
        /// Init when instantiate 
        /// </summary>
        /// <param name="coinAdded">Amount to display</param>
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
