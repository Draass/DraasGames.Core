using System.Collections.Generic;
using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.ExtensionComponents
{
    /// <summary>
    /// Base class for all components which contain extension classes inside
    /// </summary>
    public abstract class ExtensionContainer<T> : MonoBehaviour where T:  ExtensionComponent
    {
        [SerializeReference] protected List<T> _extensions = new List<T>();

        protected virtual void Start()
        {
            _extensions.ForEach(e => e.Initialize());
        }
    }
}