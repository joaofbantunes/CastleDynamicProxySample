using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodingMilitia.TimingSampleBenchmarksApplication.Service
{
    public class StuffService : IStuffService
    {
        public void DoTimeConsumingStuff()
        {
            //Do slow stuff
        }

        public Task DoTimeConsumingStuffAsync()
        {
            return Task.CompletedTask;
        }

        public Task<int> DoTimeConsumingStuffAndGetAsync()
        {
            return Task.FromResult(1);
        }
    }
}