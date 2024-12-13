using System;

namespace DraasGames.Core.Runtime.Infrastructure.StateMachine
{
    public interface IStateMachineInitializer<T> : IDisposable
    {
        public void AddState(T stateType, IStateAsync<T> stateAsync);
        
        /// <summary>
        /// Sets state machine start state. This method should be called after all states are added
        /// </summary>
        /// <param name="state"></param>
        public void SetStartState(T state);
    }
}