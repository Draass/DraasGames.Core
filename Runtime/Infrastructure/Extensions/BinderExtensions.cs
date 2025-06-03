using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Extensions
{
    public static class BinderExtensions
    {
        public static T MoveIntoAllSubContainersConditional<T>(this T binder, bool condition)
            where T : ConcreteIdArgConditionCopyNonLazyBinder
        {
            if (condition)
            {
                binder.MoveIntoAllSubContainers();
            }
            
            return binder;
        }
    }
}