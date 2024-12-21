using System;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.Effects.Abstract
{
    [Serializable]
    public abstract class NonMonoEffect
    {
        public abstract UniTask Play(bool reverse = false);
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Reset();
    }
}