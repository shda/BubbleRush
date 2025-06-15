using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace BubbleRush.Scripts
{
    public class ScoreLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshPro scoreLabel;

        private GameStats _gameStats;

        [Inject]
        private void Initialize(GameStats gameStats)
        {
            _gameStats = gameStats;
        }

        private void Awake()
        {
            _gameStats.Score.Subscribe(i => { scoreLabel.text = i.ToString(); }).AddTo(this);
        }
    }
}