using System.Text.Json;
using System.Xml.Linq;
using Avesta.Application.Interfaces;
using Avesta.Domain.ViewModels.Services;

namespace Avesta.Application.Repository
{
    public class ServiceRepository : IService
    {
        private IConfiguration _iconfiguration;
        private static readonly HttpClient client = new HttpClient();
        public ServiceRepository(IConfiguration iConfig)
        {
            _iconfiguration = iConfig;
        }

        private async Task<List<FiatGetInfoResponse>> GetFiatPrice()
        {
            List<FiatGetInfoResponse> result = new List<FiatGetInfoResponse>();
            string url = _iconfiguration["ExchangeRate:Address"] + "access_key=" + _iconfiguration["ExchangeRate:Key"] + "&symbols=" + _iconfiguration["ExchangeRate:Fiat"];
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");

                HttpResponseMessage response = await client.SendAsync(request);
                int statusCode = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement dataArray = root.GetProperty("rates");
                        foreach (JsonProperty fiat in dataArray.EnumerateObject())
                        {
                            string name = "";
                            decimal price = 0;
                            if(fiat.Name == "EUR")
                            {
                                name = fiat.Name;
                                price = Convert.ToDecimal(dataArray.GetProperty("USD").ToString());
                            }else if(fiat.Name == "USD")
                            {
                                continue;
                            }
                            else
                            {
                                name = fiat.Name;
                                decimal eurprice = Convert.ToDecimal(dataArray.GetProperty("USD").ToString());
                                price = Convert.ToDecimal(fiat.Value.ToString()) / eurprice;
                            }
                            result.Add(new FiatGetInfoResponse()
                            {
                                Name = name,
                                PricePerUsd = price,
                            });
                        }
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        private async Task<List<CoinGetInfoResponse>> GetCoinPrice(string symbol)
        {
            List<CoinGetInfoResponse> result = new List<CoinGetInfoResponse>();
            string url = _iconfiguration["CoinMarketCap:Address"] + symbol;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("x-cmc_pro_api_key", _iconfiguration["CoinMarketCap:Key"]);

                HttpResponseMessage response = await client.SendAsync(request);
                int statusCode = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement dataArray = root.GetProperty("data");
                        foreach(JsonElement coin in dataArray.EnumerateArray())
                        {
                            JsonElement priceArray = coin.GetProperty("quote")[0];
                            var i = priceArray.GetProperty("price").ToString();
                            if (priceArray.GetProperty("price").ToString() != "")
                            {
                                string name = coin.GetProperty("name").ToString() != "" ? coin.GetProperty("name").ToString() : "";
                                string symbol_res = coin.GetProperty("symbol").ToString() != "" ? coin.GetProperty("symbol").ToString() : "";
                                decimal price = Convert.ToDecimal(priceArray.GetProperty("price").ToString());
                                result.Add(new CoinGetInfoResponse()
                                {
                                    Name = name,
                                    Symbol = symbol_res,
                                    PricePerUSD = price,
                                });
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public async Task<List<ResultCoinPricesViewModel>> GetPrices(string symbol)
        {
            List<ResultCoinPricesViewModel> results = new List<ResultCoinPricesViewModel>();
            List<CoinGetInfoResponse> coinPrice = await GetCoinPrice(symbol);
            if(coinPrice.Count == 0)
            {
                return results;
            }
            List<FiatGetInfoResponse> fiatPrice = await GetFiatPrice();

            foreach(var coin in coinPrice)
            {
                List<CoinPriceDeatilsPerFiat> coinPriceDeatilsPerFiats = new List<CoinPriceDeatilsPerFiat>();
                coinPriceDeatilsPerFiats.Add(new CoinPriceDeatilsPerFiat()
                {
                    FiatSymbol = "USD",
                    Price = coin.PricePerUSD,
                });
                foreach(var fiat in fiatPrice)
                {
                    coinPriceDeatilsPerFiats.Add(new CoinPriceDeatilsPerFiat()
                    {
                        FiatSymbol = fiat.Name,
                        Price = coin.PricePerUSD * fiat.PricePerUsd,
                    });
                }
                results.Add(new ResultCoinPricesViewModel()
                {
                    CoinName = coin.Name,
                    CoinSymbol = coin.Symbol,
                    Prices = coinPriceDeatilsPerFiats,
                });
            }
            return results;
        }
    }
}
