namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewPathRetrieveStrategy
    {
        public string RetrieveViewPath(IView view);
    }
}