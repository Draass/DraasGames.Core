using System.Threading;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class Lifetime : ILifetime
    {
        private CancellationTokenSource _cts = new();

        public CancellationToken Token => _cts.Token;
        
        public async UniTask WaitForEnd()
        {
            await Token.WaitUntilCanceled();
        }

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