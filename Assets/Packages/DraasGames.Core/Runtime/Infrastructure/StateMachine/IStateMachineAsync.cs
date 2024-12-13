using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.Infrastructure.StateMachine
{
    public interface IStateMachineAsync<T>
    {
        public UniTask Enter(T state);
        
        public UniTask Initialize();
    }
}