namespace CodeBase.Infrastructure
{
    public class LoadProgressState : IState
    {
        private const string Main = "Main";
        
        private readonly IGameStateMachine _gameStateMachine;

        public LoadProgressState(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            _gameStateMachine.Enter<LoadLevelState, string>(Main);
        }

        public void Exit()
        {
        }
    }
}