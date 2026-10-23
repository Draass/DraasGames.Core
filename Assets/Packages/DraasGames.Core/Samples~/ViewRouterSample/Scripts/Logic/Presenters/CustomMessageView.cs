using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Concrete;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal class CustomMessageView : View<string>
    {
        public override UniTask ShowAsync(string message)
        {
            Initialize(message);
            return base.ShowAsync(message);
        }

        private void Initialize(string message)
        {
        }
    }
}
