using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.UI.Intro
{
    public class IntroUI : MonoBehaviour
    {
        public GameObject closing;
        public float fakeTime;

        [Space] [SerializeField] private AnimationClip _closingAnim;

        private void Awake()
        {
            closing.SetActive(false);
        }

        private void Start()
        {
            StartCoroutine(FakeLoading(fakeTime));
        }

        IEnumerator FakeLoading(float amount)
        {
            yield return new WaitForSeconds(amount);
        
            closing.SetActive(true);

            yield return new WaitForSeconds(_closingAnim.length + 1.1f);
            SceneManager.LoadScene("Menu");
        }
    }
}
