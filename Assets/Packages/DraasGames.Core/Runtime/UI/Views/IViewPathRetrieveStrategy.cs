using DraasGames.Core.Runtime.UI.Views.Abstract;

namespace DraasGames.Core.Runtime.UI.Views
{
    public interface IViewPathRetrieveStrategy
    {
        public string RetrieveViewPath(IView view);
    }
}