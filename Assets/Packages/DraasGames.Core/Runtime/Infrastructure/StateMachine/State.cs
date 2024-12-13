using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.Infrastructure.StateMachine
{
    public abstract class State<T> : IStateAsync<T>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async UniTask OnEnter() { }
        
        public virtual async UniTask OnExit() { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}