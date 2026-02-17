using UnityEngine;

namespace DraasGames.Core.Runtime.Pooling
{
    public class PoolTransformProvider : MonoBehaviour, IPoolTransformProvider
    {
        [field: SerializeField]
        public Transform PoolContainer { get; private set; }
    }
}