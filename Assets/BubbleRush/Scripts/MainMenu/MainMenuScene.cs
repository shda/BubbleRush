using System;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleRush.Scripts.MainMenu
{
    public class MainMenuScene : MonoBehaviour
    {
        [SerializeField] private GameStats gameStats;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private SceneReference gameSceneReference;
        private void Start()
        {
            gameStats.LoadValues();
            bestScoreText.text = gameStats.BestScore.Value.ToString();
        }

        public void OnPressStart()
        {
            SceneManager.LoadScene(gameSceneReference.Name);
        }
    }
}