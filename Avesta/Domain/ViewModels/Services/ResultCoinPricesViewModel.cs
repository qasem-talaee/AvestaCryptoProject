namespace Avesta.Domain.ViewModels.Services
{
    public class ResultCoinPricesViewModel
    {
        public string CoinName { get; set; } = string.Empty;
        public string CoinSymbol { get; set; } = string.Empty;
        public List<CoinPriceDeatilsPerFiat> Prices { get; set; }
    }
    public class CoinPriceDeatilsPerFiat
    {
        public string FiatSymbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
