namespace CodeBase.Infrastructure
{
    public interface IGameStateMachine
    {
        void Setup();
        void Enter<TState>() where TState : IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : IPayloadedState<TPayload>;
    }
}