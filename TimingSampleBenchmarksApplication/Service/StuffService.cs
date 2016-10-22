using System;
using System.Threading;

namespace CodingMilitia.TimingSampleBenchmarksApplication.Service
{
    public class StuffService : IStuffService
    {
        public void DoTimeConsumingStuff()
        {
            //Do slow stuff
        }
    }
}