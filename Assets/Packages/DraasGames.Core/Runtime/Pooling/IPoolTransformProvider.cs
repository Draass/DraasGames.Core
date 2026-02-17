using UnityEngine;

namespace DraasGames.Core.Runtime.Pooling
{
    public interface IPoolTransformProvider
    {
        Transform PoolContainer { get; }
    }
}