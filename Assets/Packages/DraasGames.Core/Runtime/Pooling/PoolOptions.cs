using System;
using UnityEngine;

namespace DraasGames.Core.Runtime.Pooling
{
    public record PoolOptions<T> where T: Component, IPoolable
    {
        public Func<T> CreateFunc = null;
        public Action<T> ActionOnGet = null;
        public Action<T> ActionOnRelease = null;
        public Action<T> ActionOnDestroy = null;
        public bool CollectionCheck = true;
        public int DefaultCapacity = 10;
        public int MaxSize = 1000;
        public PoolType PoolType = PoolType.Stack;
    }
}