using System;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusController : MonoBehaviour
    {
        [SerializeField] private BonusRay bonusRay;
        [SerializeField] private BonusExplosion bonusExplosion;
        
        private BonusBase _currentBonus;
        
        private void StartBonus(BonusBase bonus)
        {
            _currentBonus?.DisableBonus();
            _currentBonus = bonus;
            _currentBonus.OnCompleteBonus = OnCompleteBonus;
            _currentBonus.EnableBonus();
        }

        private void OnCompleteBonus(BonusBase bonus)
        {
            bonus.OnCompleteBonus = null;
            _currentBonus?.DisableBonus();
            _currentBonus = null;
        }

        public async UniTask<bool> CompleteShotBubbleOnTarget(Bubble projectile)
        {
            return _currentBonus != null && await _currentBonus.CompleteShotBubbleOnTarget(projectile);
        }

        public void StartShot()
        {
            _currentBonus?.StartShot();
        }
        
        [Button]
        public void EnableRayBonus()
        {
            StartBonus(bonusRay);
        }
        
        [Button]
        public void EnableBonusExplosion()
        {
            StartBonus(bonusExplosion);
        }
    }
}