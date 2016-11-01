using System;
using System.Threading.Tasks;

namespace CodingMilitia.TimingSampleBenchmarksApplication.Service
{
    public interface IStuffService
    {
        void DoTimeConsumingStuff();

        Task DoTimeConsumingStuffAsync();

        Task<int> DoTimeConsumingStuffAndGetAsync();
    }
}