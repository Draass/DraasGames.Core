using System.Threading;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class Lifetime : ILifetime
    {
        private CancellationTokenSource _cts = new();

        public CancellationToken Token => _cts.Token;

        public void Dispose()
        {
            if (_cts == null)
            {
                return;
            }

            try
            {
                if (!_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}