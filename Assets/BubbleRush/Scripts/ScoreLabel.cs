using TMPro;
using UniRx;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class ScoreLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshPro scoreLabel;
        [SerializeField] private GameStats gameStats;

        private void Awake()
        {
            gameStats.Score.Subscribe(i =>
            {
                scoreLabel.text = i.ToString();
            }).AddTo(this);
        }
    }
}