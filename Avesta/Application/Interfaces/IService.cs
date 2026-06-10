using Avesta.Domain.ViewModels.Services;

namespace Avesta.Application.Interfaces
{
    public interface IService
    {
        Task<List<ResultCoinPricesViewModel>> GetPrices(string symbol);
    }
}
