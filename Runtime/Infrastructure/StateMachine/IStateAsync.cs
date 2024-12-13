using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.Infrastructure.StateMachine
{
    public interface IStateAsync<T>
    {
        public UniTask OnEnter();
        public UniTask OnExit();
    }
}