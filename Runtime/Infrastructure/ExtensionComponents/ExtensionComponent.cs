using System;
using DraasGames.Core.Runtime.Infrastructure.Core;

namespace DraasGames.Core.Runtime.Infrastructure.ExtensionComponents
{
    /// <summary>
    /// Base abstract class used for assigning components to different behavioral monobehaviour systems
    /// </summary>
    [Serializable]
    public abstract class ExtensionComponent : IInitializable
    {
        public virtual void Initialize() { }
    }
}