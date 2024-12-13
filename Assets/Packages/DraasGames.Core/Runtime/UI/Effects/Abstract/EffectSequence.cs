using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Effects.Abstract
{
    public class EffectSequence : MonoBehaviour, IEffectSequence
    {
        [SerializeReference] 
        private List<Effect> _effectsSequence = new List<Effect>();
        
        [Button]        
        public void PlaySequence(bool sequentially = true)
        {
            if (sequentially)
            {
                PlaySequential().Forget();
            }
            else
            {
                foreach (var effect in _effectsSequence)
                {
                    effect.Play().Forget();
                }
            }
        }

        public void CancelSequence()
        {
        }

        private async UniTaskVoid PlaySequential()
        {
            foreach (var effect in _effectsSequence)
            {
                await effect.Play();
            }
        }

        public void AddEffectToSequence(Effect effect)
        {
            _effectsSequence.Add(effect);
        }

        public void RemoveEffect(Effect effect)
        {
            if(_effectsSequence.Contains(effect))
                _effectsSequence.Remove(effect);
        }
    }
}