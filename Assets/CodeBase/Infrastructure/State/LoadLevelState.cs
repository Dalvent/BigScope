using CodeBase.Services;

namespace CodeBase.Infrastructure
{
    public class LoadLevelState  : IPayloadedState<string>
    {
        private readonly IGameFactory _gameFactory;
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameStateMachine _gameStateMachine;

        public LoadLevelState(IGameFactory gameFactory, ISceneLoader sceneLoader, IGameStateMachine gameStateMachine)
        {
            _gameFactory = gameFactory;
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
        }

        public void Enter(string payload)
        {;
            _sceneLoader.Load(payload, OnLoad);
        }

        public void Exit()
        {
        }
        
        private async void OnLoad()
        {
            _gameStateMachine.Enter<GameLoopState>();
        }
    }
}