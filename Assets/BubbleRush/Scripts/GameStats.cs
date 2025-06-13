using UniRx;
using UnityEngine;

namespace BubbleRush.Scripts
{
    [CreateAssetMenu(menuName = "Create GameStats", fileName = "GameStats", order = 0)]
    public class GameStats : ScriptableObject
    {
        public IntReactiveProperty BestScore { get;set; } = new();
        public IntReactiveProperty Score { get;set; } = new();
        public BoolReactiveProperty BonusRay { get;set; } = new(true);
        public BoolReactiveProperty BonusExplosion { get;set; } = new(true);

        public void Init()
        {
            Score.Value = 0;
            BonusRay.Value = true;
            BonusExplosion.Value = true;
        }
        
        public void LoadValues()
        {
            BestScore.Value = PlayerPrefs.GetInt(nameof(BestScore));
        }
        
        public void SaveValues()
        {
            if(Score.Value > BestScore.Value)
            {
                BestScore.Value = Score.Value;
            }
            PlayerPrefs.SetInt(nameof(BestScore), BestScore.Value);
        }
    }
}