using System;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BubbleRush.Scripts.MainMenu
{
    public class MainMenuScene : MonoBehaviour
    {
       // [SerializeField] private GameStats gameStats;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private SceneReference gameSceneReference;
        private GameStats _gameStats;
        
        [Inject]
        private void Initialize(GameStats gameStats)
        {
            _gameStats = gameStats;
        }
        private void Start()
        {
            _gameStats.LoadValues();
            bestScoreText.text = _gameStats.BestScore.Value.ToString();
        }

        public void OnPressStart()
        {
            SceneManager.LoadScene(gameSceneReference.Name);
        }
    }
}