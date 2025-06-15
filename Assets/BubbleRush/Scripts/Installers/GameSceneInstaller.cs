using BubbleRush.Scripts.Services;
using BubbleRush.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace BubbleRush.Scripts.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private Bubble bubblePrefab;
        [SerializeField] private Camera mainCamera;
        public override void InstallBindings()
        {
            BindSettings();
            BindBubbleFactory();
            
            Container.BindInstance(mainCamera);

            Container.Bind<BubblesAnimationService>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
            Container.Bind<IGameFieldMaker>().To<GameFieldMaker>().AsSingle();
        }

        private void BindSettings()
        {
            Container.BindInstance(gameSettings);
        }
        
        private void BindBubbleFactory()
        {
            Container.BindFactory<Bubble, Scripts.BubblePool>().FromPoolableMemoryPool<Bubble, BubblePool>(poolBinder => 
                poolBinder
                    .WithInitialSize(100)
                    .FromComponentInNewPrefab(bubblePrefab)
                    .UnderTransformGroup("BubblesPool"));
        }
        
        private class BubblePool : MonoPoolableMemoryPool<IMemoryPool, Bubble> { }
    }
}
