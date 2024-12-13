using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Effects.Abstract
{
    public abstract class Effect : MonoBehaviour
    {
        public abstract UniTask Play(bool reverse = false);
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Reset();
    }
}