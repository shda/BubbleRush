using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusRay : BonusBase
    {
        [SerializeField] private BubbleRay bubbleRay;
        [SerializeField] private int maxCountShots = 5;
        
        private int _currentShot;
            
        public override void EnableBonus()
        {
            bubbleRay.StartDrawLine();
            _currentShot = 0;
        }

        public override void DisableBonus()
        {
            bubbleRay.StopDrawLine();
        }

        public override async UniTask<bool> CompleteShotBubbleOnTarget(Bubble bubble)
        {
            if (_currentShot < maxCountShots)
            {
                bubbleRay.StartDrawLine();
            }
            else
            {
                bubbleRay.StopDrawLine();
                OnCompleteBonus?.Invoke(this);
            }
            
            return false;
        }

        public override void StartShot()
        {
            _currentShot++;
            bubbleRay.StopDrawLine();
            base.StartShot();
        }
    }
}