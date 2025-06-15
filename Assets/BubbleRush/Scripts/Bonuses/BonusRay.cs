using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusRay : BonusBase
    {
        [SerializeField] private SimulationBubbleTrajectory simulationBubbleTrajectory;
        [SerializeField] private int disableRayAfterShots = 5;
        
        private int _currentShot;
            
        public override void EnableBonus()
        {
            simulationBubbleTrajectory.StartDrawLine();
            _currentShot = 0;
        }

        public override void DisableBonus()
        {
            simulationBubbleTrajectory.StopDrawLine();
        }

        public override async UniTask<bool> CompleteShotBubbleOnTarget(Bubble bubble)
        {
            if (_currentShot < disableRayAfterShots)
            {
                simulationBubbleTrajectory.StartDrawLine();
            }
            else
            {
                simulationBubbleTrajectory.StopDrawLine();
                OnCompleteBonus?.Invoke(this);
            }
            
            return false;
        }

        public override void StartShot()
        {
            _currentShot++;
            simulationBubbleTrajectory.StopDrawLine();
            base.StartShot();
        }
    }
}