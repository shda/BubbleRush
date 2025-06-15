using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleRush.Scripts.GameUi
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameStats gameStats;

        private void OnEnable()
        {
            scoreText.text = gameStats.Score.Value.ToString();
        }

        public void OnPressRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}