namespace Avesta.Domain.ViewModels.Services
{
    public class FiatGetInfoResponse
    {
        public string Name { get; set; } = string.Empty;
        public decimal PricePerUsd { get; set; }
    }
}
