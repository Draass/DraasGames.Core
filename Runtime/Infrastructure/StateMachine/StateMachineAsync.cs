using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.StateMachine
{
    public abstract class StateMachineAsync<TState> : IStateMachineAsync<TState>, IStateMachineInitializer<TState>
    {
        private readonly Dictionary<TState, IStateAsync<TState>> _states = new();
     
        private IStateAsync<TState> _currentState = null;
        
        private IStateAsync<TState> _startState;
        private TState _startStateType;
        
        public virtual async UniTask Enter(TState state)
        {
            if (_currentState == null)
            {
                Debug.LogError("Current state is not set");
                return;
            }
            
            Debug.Log($"Exiting state {_currentState.GetType().Name}");
            await _currentState.OnExit();
            
            if(!_states.TryGetValue(state, out _currentState))
            {
                //or just log error and return?
                throw new Exception($"State {state} does not exist");
            }
            
            _currentState = _states[state];
            
            Debug.Log($"Entering state {_currentState.GetType().Name}");
            await _currentState.OnEnter();
        }

        /// <summary>
        /// State machine-only method for entering start state
        /// </summary>
        /// <param name="state"></param>
        private async UniTask Enter(IStateAsync<TState> state)
        {
            _currentState = state;
            
            await state.OnEnter();
        }

        public virtual async UniTask Initialize()
        {
            if (_startState == null)
            {
                Debug.LogError("Start state is not set");
                return;
            }
            
            await Enter(_startState);
            
            Debug.Log("State machine initialized");
        }

        public void AddState(TState stateType, IStateAsync<TState> stateAsync)
        {
            if(!_states.TryAdd(stateType, stateAsync))
            {
                Debug.LogWarning($"State {stateType} already exists");
            }
        }

        public void SetStartState(TState state)
        {
            if(!_states.ContainsKey(state))
            {
                Debug.LogWarning($"State {state} does not exist");
                return;
            }
            
            _startState = _states[state];
        }

        public void Dispose()
        {
            _currentState = null;
            _startState = null;
            _states.Clear();
        }
    }
}