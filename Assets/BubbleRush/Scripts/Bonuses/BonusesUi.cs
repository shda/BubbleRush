using System;
using BubbleRush.Scripts.GameUi;
using UniRx;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusesUi : MonoBehaviour
    {
        [SerializeField] private GameStats gameStats;
        [SerializeField] private ButtonUi buttonRay;
        [SerializeField] private ButtonUi buttonExplosion;
        [SerializeField] private BonusController bonusController;

        private void Awake()
        {
            gameStats.BonusRay.Subscribe(buttonRay.gameObject.SetActive).AddTo(this);
            gameStats.BonusExplosion.Subscribe(buttonExplosion.gameObject.SetActive).AddTo(this);
            
            buttonRay.OnPressButton = OnStartBonusRay;
            buttonExplosion.OnPressButton = OnStarBonusExplosion;
        }

        private void OnStartBonusRay(ButtonUi button)
        {
            gameStats.BonusRay.Value = false;
            bonusController.EnableRayBonus();
        }
        
        private void OnStarBonusExplosion(ButtonUi button)
        {
            gameStats.BonusExplosion.Value = false;
            bonusController.EnableBonusExplosion();
        }
    }
}