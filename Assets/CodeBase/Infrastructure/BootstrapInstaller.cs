using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner, IInitializable
    {
        public override void InstallBindings()
        {
            BindInstallersInterface();
            
            BindInput();
            BindSceneLoader();
            BindGameStateMachine();
            BindGameFactory();
        }

        public void Initialize()
        {
            IGameStateMachine gameStateMachine = Container.Resolve<IGameStateMachine>();
            gameStateMachine.Setup();
            gameStateMachine.Enter<BootstrapState>();
        }

        private void BindInstallersInterface()
        {
            Container.BindInterfacesTo<BootstrapInstaller>()
                .FromInstance(this)
                .AsSingle();
        }

        private void BindGameStateMachine()
        {
            Container.Bind<IGameStateMachine>()
                .To<GameStateMachine>()
                .AsSingle();
        }

        private void BindSceneLoader()
        {
            Container.Bind<ISceneLoader>()
                .To<SceneLoader>()
                .AsSingle();
        }
        
        private void BindGameFactory()
        {
            Container.Bind<IGameFactory>()
                .To<GameFactory>()
                .AsSingle();
        }

        private void BindInput()
        {
            Container.Bind<IInputService>()
                .To<InputService>()
                .AsSingle();
        }
    }
}