using System;
using BubbleRush.Scripts.GameUi;
using UniRx;
using UnityEngine;
using Zenject;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusesUi : MonoBehaviour
    {
      //  [SerializeField] private GameStats gameStats;
        [SerializeField] private ButtonUi buttonRay;
        [SerializeField] private ButtonUi buttonExplosion;
        [SerializeField] private BonusController bonusController;

        private GameStats _gameStats;
        
        [Inject]
        private void Initialize(GameStats gameStats)
        {
            _gameStats = gameStats;
        }
        
        private void Awake()
        {
            _gameStats.BonusRay.Subscribe(buttonRay.gameObject.SetActive).AddTo(this);
            _gameStats.BonusExplosion.Subscribe(buttonExplosion.gameObject.SetActive).AddTo(this);
            
            buttonRay.OnPressButton = OnStartBonusRay;
            buttonExplosion.OnPressButton = OnStarBonusExplosion;
        }

        private void OnStartBonusRay(ButtonUi button)
        {
            _gameStats.BonusRay.Value = false;
            bonusController.EnableRayBonus();
        }
        
        private void OnStarBonusExplosion(ButtonUi button)
        {
            _gameStats.BonusExplosion.Value = false;
            bonusController.EnableBonusExplosion();
        }
    }
}