using Zenject;

namespace BubbleRush.Scripts.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStats>().AsSingle();
        }
    }
}