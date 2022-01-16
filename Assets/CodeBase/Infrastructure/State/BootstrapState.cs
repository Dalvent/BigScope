using CodeBase.Services;

namespace CodeBase.Infrastructure
{
    public class BootstrapState : IState
    {
        private const string Bootstrap = "Bootstrap";
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameStateMachine _stateMachine;

        public BootstrapState(ISceneLoader sceneLoader, IGameStateMachine stateMachine)
        {
            _sceneLoader = sceneLoader;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Enter<LoadProgressState>();
        }

        public void Exit()
        {
        }
    }
}