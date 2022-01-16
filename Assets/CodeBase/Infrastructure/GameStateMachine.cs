using System;
using System.Collections.Generic;
using Zenject;

namespace CodeBase.Infrastructure
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly DiContainer _container;

        private Dictionary<Type, IExitableState> _state;
        private IExitableState _activeState;

        public GameStateMachine(DiContainer container)
        {
            _container = container;
        }

        public void Setup()
        {
            _state = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = _container.Instantiate<BootstrapState>(),
                [typeof(LoadLevelState)] = _container.Instantiate<LoadLevelState>(),
                [typeof(LoadProgressState)] = _container.Instantiate<LoadProgressState>(),
                [typeof(GameLoopState)] = _container.Instantiate<GameLoopState>()
            };
        }

        public void Enter<TState>() where TState : IState
        {
            var state = EnterState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : IPayloadedState<TPayload>
        {
            var state = EnterState<TState>();
            state.Enter(payload);
        }

        private TState EnterState<TState>() where TState : IExitableState
        {
            _activeState?.Exit();
            TState state = (TState)_state[typeof(TState)];
            _activeState = state;
            return state;
        }
    }
}