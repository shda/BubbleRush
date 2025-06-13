using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public abstract class BonusBase : MonoBehaviour
    {
        public Action<BonusBase> OnCompleteBonus;

        public virtual void EnableBonus()
        {
          
        }

        public virtual void DisableBonus()
        {
            
        }

        public virtual void StartShot()
        {
            
        }
        public virtual async UniTask<bool> CompleteShotBubbleOnTarget(Bubble bubble)
        {
            return true;
        }
    }
}