using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BubbleRush.Scripts.GameUi
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private GameStats _gameStats;

        [Inject]
        private void Initialize(GameStats gameStats)
        {
            _gameStats = gameStats;
        }

        private void OnEnable()
        {
            scoreText.text = _gameStats.Score.Value.ToString();
        }

        public void OnPressRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}