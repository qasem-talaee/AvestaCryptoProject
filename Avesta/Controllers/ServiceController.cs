using Avesta.Application.Interfaces;
using Avesta.Domain.ViewModels.Services;
using Microsoft.AspNetCore.Mvc;

namespace Avesta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IService _iService;
        public ServiceController(IService iService)
        {
            _iService = iService;
        }

        [HttpGet("GetPrices/{symbol}")]
        public async Task<List<ResultCoinPricesViewModel>> GetPrices(string symbol)
        {
            return await _iService.GetPrices(symbol);
        }
    }
}
