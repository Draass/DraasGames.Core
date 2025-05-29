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
            var cts = Interlocked.Exchange(ref _cts, null);

            if (cts == null)
            {
                return;
            }

            try
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}