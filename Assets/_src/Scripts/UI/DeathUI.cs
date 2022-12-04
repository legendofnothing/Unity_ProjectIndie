using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _src.Scripts.UI
{
    public class DeathUI : MonoBehaviour
    {
        public LevelData previousLevelData;

        [Header("Text")] 
        public TextMeshProUGUI highScoreNum;
        public TextMeshProUGUI turnNum;

        private void Start()
        {
            Time.timeScale = 0;
            gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

            highScoreNum.text = $"{previousLevelData.score}";
            turnNum.text = $"{previousLevelData.turnNumber}";
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("Menu");
            Time.timeScale = 1;
        }

        public void Retry()
        {
            SceneManager.LoadScene(previousLevelData.sceneIndex);
            Time.timeScale = 1;
        }
    }
}
