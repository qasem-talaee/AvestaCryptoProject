namespace Avesta.Domain.ViewModels.Services
{
    public class CoinGetInfoResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal PricePerUSD {  get; set; }
    }
}
