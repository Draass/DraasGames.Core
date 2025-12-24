using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using UnityEditor;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public static class ProjectLifetimeHolder
    {
        public static ILifetime ProjectLifeTime { get; private set; } = new Lifetime();
        
        #if UNITY_EDITOR
                [InitializeOnEnterPlayMode]
                private static void EditorInit()
                {
                    ProjectLifeTime = new Lifetime();
                }
        #endif

    }
}