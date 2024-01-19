using AngleSharp.Dom;
using SwarmBot.Application;
using SwarmBot.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace SwarmBot.Infrastructure.Services
{
    internal class ScToolsService : IScToolsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _scToolsbaseUrlRsi;
        private readonly ILogger _logger;
        private readonly string _defaultFilePath = "/app/data/rsiData.json";
        private readonly string _defaultPath = "/app/data";
        private readonly IDictionary<string, string> _shipNameDictionary = new Dictionary<string, string>{
            { "Hornet", "F7C Hornet" },
            { "Hornet F7C", "F7C Hornet" },
            { "Ursa Rover", "Ursa" }
        };

        public ScToolsService(IHttpClientFactory aHttpClientFactory, IConfiguration aConfiguration, ILogger<ScToolsService> aLogger)
        {
            _httpClientFactory = aHttpClientFactory;
            _logger = aLogger;
            _scToolsbaseUrlRsi = aConfiguration.GetValue<string>("ScToolsbaseUrlRsi")
                ?? throw new ArgumentNullException("Failed on fetching 'ScToolsbaseUrlRsi' from config, a value is required.");
        }

        public async Task<IHttpResult<List<Ship>>> GetRsiShipList()
        {
            var json = await File.ReadAllTextAsync(_defaultFilePath);
            var data = System.Text.Json.JsonSerializer.Deserialize<List<Ship>>(json);
            return Result.SuccessHttp(data)!;
        }

        public async Task GetRsiData()
        {
            string RsiAuthToken = await GetRsiAuthToken();

            List<Ship> ListShip = new List<Ship> { new Ship() {
                Id = "ptv",
                Name = "PTV Buggy",
                Price = 0,
                FlyableStatus = "flight-ready",
                Images = new ShipImages()
                {
                    Small = "",
                    Medium = "",
                },
                Manufacturer = new ShipManufacturer()
                {
                    Id = "17",
                    Name = "Greycat Industrial"
                },
                Focus = "Transport",
                Type = "ground",
                Link = "",
                CcuList = new List<ShipCcu>(),
                StandaloneList = new List<ShipStandalone>()
            }};

            JToken ShipMatrixData = await GetRsiShipMatrixData(RsiAuthToken);
            ShipMatrixData.ForEach(dataShip =>
            {
                string Id = dataShip["id"]!.ToString();
                string Name = dataShip["name"]!.ToString();
                float Price = 0;
                ListShip.Add(
                    new Ship()
                    {
                        Id = Id,
                        Name = Name,
                        Price = Price,
                        FlyableStatus = dataShip["production_status"]!.ToString(),
                        Images = new ShipImages()
                        {
                            Small = dataShip["media"]![0]!["images"]!["store_small"]!.ToString(),
                            Medium = dataShip["media"]![0]!["images"]!["store_large"]!.ToString(),
                        },
                        Manufacturer = new ShipManufacturer()
                        {
                            Id = dataShip["manufacturer"]!["id"]!.ToString(),
                            Name = dataShip["manufacturer"]!["name"]!.ToString()
                        },
                        Focus = dataShip["focus"]!.ToString(),
                        Type = dataShip["type"]!.ToString(),
                        Link = dataShip["url"]!.ToString(),
                        CcuList = new List<ShipCcu>(),
                        StandaloneList = new List<ShipStandalone>()
                    });
            });

            JToken ShipCcuData = await GetRsiShipCcuData(RsiAuthToken);

            ShipCcuData["from"]!["ships"]!.ForEach(dataShip =>
            {
                var index = ListShip.FindIndex(Ship => Ship.Id == dataShip["id"]!.ToString());
                ListShip[index].Price = NormalizePrice(dataShip["msrp"]!.ToString());
            });

            ShipCcuData["to"]!["ships"]!.ForEach(dataShip =>
            {
                var index = ListShip.FindIndex(Ship => Ship.Id == dataShip["id"]!.ToString());
                foreach (var CcuData in dataShip["skus"]!)
                {
                    string CcuId = CcuData["id"]!.ToString();
                    string CcuName = CcuData["title"]!.ToString();
                    float CcuPrice = NormalizePrice(CcuData["price"]!.ToString());
                    ListShip[index].CcuList.Add(new ShipCcu() { Id = CcuId, Name = CcuName, Price = CcuPrice });
                }
            });

            JToken ShipStandaloneData = await GetRsiShipStandaloneData(RsiAuthToken);

            ShipStandaloneData.ForEach(standalone => {
                
                string ShipName = Convert.ToBoolean(standalone["isPackage"]!.ToString()) ? NormalizePackageName(standalone["name"]!.ToString()) : NormalizeShipName(standalone["name"]!.ToString());
                var index = ListShip.FindIndex(Ship => Ship.Name.ToLower().Trim() == ShipName.ToLower().Trim());

                if (ShipName == "PTV Buggy") {
                    ListShip[index].Price = NormalizePrice(standalone["nativePrice"]!["amount"]!.ToString());
                    ListShip[index].Link = standalone["url"]!.ToString();
                    ListShip[index].Images = new ShipImages()
                    {
                        Small = standalone["media"]!["thumbnail"]!["storeSmall"]!.ToString(),
                        Medium = standalone["media"]!["thumbnail"]!["slideshow"]!.ToString(),
                    };
                }

                if (index >= 0)
                {
                    string taxDescription = NormalizeTaxDescription(standalone["price"]!["taxDescription"]!.ToString());
                    ListShip[index].StandaloneList.Add(
                        new ShipStandalone()
                        {
                            Id = standalone["id"]!.ToString(),
                            Name = standalone["name"]!.ToString(),
                            Title = standalone["title"]!.ToString(),
                            Subtitle = standalone["subtitle"]!.ToString(),
                            Url = standalone["url"]!.ToString(),
                            Body = standalone["body"]!.ToString(),
                            Excerpt = standalone["excerpt"]!.ToString(),
                            Type = standalone["type"]!.ToString(),
                            Images = new ShipImages()
                            {
                                Small = standalone["media"]!["thumbnail"]!["storeSmall"]!.ToString(),
                                Medium = standalone["media"]!["thumbnail"]!["slideshow"]!.ToString(),
                            },
                            Price = NormalizePrice(standalone["nativePrice"]!["amount"]!.ToString()),
                            PriceWithTax = NormalizePrice(standalone["price"]!["amount"]!.ToString()),
                            TaxDescription = taxDescription,
                            IsWarbond = Convert.ToBoolean(standalone["isWarbond"]!.ToString()),
                            IsPackage = Convert.ToBoolean(standalone["isPackage"]!.ToString()),
                        }
                    );
                }

            });

            await SaveRsiDataOnFile(ListShip);
        }

        private async Task<string> GetRsiAuthToken()
        {
            string rsiToken = await GetRsiToken();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _scToolsbaseUrlRsi + "/api/account/v2/setAuthToken");
            httpRequestMessage.Headers.Add("x-rsi-token", rsiToken);
            HttpClient HttpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage lHttpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);
            string Content = await lHttpResponseMessage.Content.ReadAsStringAsync();
            string RsiAuthToken = JObject.Parse(Content)["data"]!.ToString();
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: RsiAuthToken");
            return RsiAuthToken;
        }

        private async Task<string> GetRsiToken()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(_scToolsbaseUrlRsi + "/pledge");
                Uri uri = new Uri(_scToolsbaseUrlRsi + "/pledge");
                var rsiToken = cookieContainer.GetCookies(uri)[0].Value;
                _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: RsiToken");
                return rsiToken.ToString();
            }
        }

        private async Task<JToken> GetRsiShipMatrixData(string RsiAuthToken) {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _scToolsbaseUrlRsi + "/ship-matrix/index");
            httpRequestMessage.Headers.Add("Cookie", "Rsi-Account-Auth=" + RsiAuthToken);
            HttpClient HttpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage lHttpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);
            string Content = await lHttpResponseMessage.Content.ReadAsStringAsync();
            JToken RsiShipMatrixData = JObject.Parse(Content)["data"]!;
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: ShipMatrixData");
            return RsiShipMatrixData;
        }

        private async Task<JToken> GetRsiShipCcuData(string RsiAuthToken) {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _scToolsbaseUrlRsi + "/pledge-store/api/upgrade/graphql");
            httpRequestMessage.Headers.Add("Cookie", "Rsi-Account-Auth=" + RsiAuthToken);
            var query = new
            {
                operationName = "filterShips",
                variables = new
                {
                    fromFilters = Array.Empty<string>(),
                    toFilters = Array.Empty<string>(),
                },
                query = "query filterShips($fromId: Int, $toId: Int, $fromFilters: [FilterConstraintValues], $toFilters: [FilterConstraintValues]) {\n from(to: $toId, filters: $fromFilters) {\n ships {\n id\n name\n medias {\n productThumbMediumAndSmall\n slideShow\n }\n manufacturer {\n id\n name\n }\n focus\n type\n flyableStatus\n msrp\n link\n skus {\n id\n title\n price\n items {\n id\n title\n}\n }\n }\n }\n to(from: $fromId, filters: $toFilters) {\n ships {\n id\n name\n flyableStatus\n msrp\n skus {\n id\n title\n price\n items {\n id\n title\n }\n }\n } \n} \n}\n",
            };
            string jsonQuery = JsonConvert.SerializeObject(query);
            httpRequestMessage.Content = new StringContent(jsonQuery, Encoding.UTF8, "application/json");
            HttpClient HttpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage lHttpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);
            string Content = await lHttpResponseMessage.Content.ReadAsStringAsync();
            JToken RsiShipCcuData = JObject.Parse(Content)["data"]!;
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: RsiShipCcuData");
            return RsiShipCcuData;
        }

        private async Task<JToken> GetRsiShipStandaloneData(string RsiAuthToken)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _scToolsbaseUrlRsi + "/graphql");
            httpRequestMessage.Headers.Add("Cookie", "Rsi-XSRF=" + RsiAuthToken);
            var query = new
            {
                operationName = "GetBrowseListingQuery",
                variables = new
                {
                    query = new
                    {
                        skus = new
                        {
                            products = new string[] { "72", "9", "45", "46" }
                        },
                        limit = 10000,
                        page = 1,
                        sort = new
                        {
                            field = "weight",
                            direction = "desc"
                        }
                    }
                },
                query = "query GetBrowseListingQuery($query: SearchQuery) {\n store(browse: true) {\n listing: search(query: $query) {\n resources {\n ...TyItemFragment\n __typename\n }\n __typename\n }\n __typename\n }\n}\n\nfragment TyItemFragment on TyItem {\n id\n slug\n name\n title\n subtitle\n url\n body\n excerpt\n type\n media {\n thumbnail {\n slideshow\n storeSmall\n __typename\n }\n list {\n slideshow\n __typename\n }\n __typename\n }\n nativePrice {\n amount\n discounted\n discountDescription\n __typename\n }\n price {\n amount\n discounted\n taxDescription\n discountDescription\n __typename\n }\n stock {\n ...TyStockFragment\n __typename\n }\n tags {\n ...TyHeapTagFragment\n __typename\n }\n ... on TySku {\n label\n customizable\n isWarbond\n isPackage\n isVip\n isDirectCheckout\n __typename\n }\n ... on TyProduct {\n skus {\n id\n title\n isDirectCheckout\n __typename\n }\n isVip\n __typename\n }\n ... on TyBundle {\n isVip\n media {\n thumbnail {\n slideshow\n __typename\n }\n __typename\n }\n discount {\n ...TyDiscountFragment\n __typename\n }\n __typename\n }\n __typename\n}\n\nfragment TyHeapTagFragment on HeapTag {\n name\n excerpt\n __typename\n}\n\nfragment TyDiscountFragment on TyDiscount {\n id\n title\n skus {\n ...TyBundleSkuFragment\n __typename\n }\n products {\n ...TyBundleProductFragment\n __typename\n }\n __typename\n}\n\nfragment TyBundleSkuFragment on TySku {\n id\n title\n label\n excerpt\n subtitle\n url\n type\n isWarbond\n isDirectCheckout\n media {\n thumbnail {\n storeSmall\n slideshow\n __typename\n }\n __typename\n }\n gameItems {\n __typename\n }\n stock {\n ...TyStockFragment\n __typename\n }\n price {\n amount\n taxDescription\n __typename\n }\n tags {\n ...TyHeapTagFragment\n __typename\n }\n __typename\n}\n\nfragment TyStockFragment on TyStock {\n unlimited\n show\n available\n backOrder\n qty\n backOrderQty\n level\n __typename\n}\n\nfragment TyBundleProductFragment on TyProduct {\n id\n name\n title\n subtitle\n url\n type\n excerpt\n stock {\n ...TyStockFragment\n __typename\n }\n media {\n thumbnail {\n storeSmall\n slideshow\n __typename\n }\n __typename\n }\n nativePrice {\n amount\n discounted\n __typename\n }\n price {\n amount\n discounted\n taxDescription\n __typename\n }\n skus {\n ...TyBundleSkuFragment\n __typename\n }\n __typename\n}\n",
            };
            string jsonQuery = JsonConvert.SerializeObject(query);
            httpRequestMessage.Content = new StringContent(jsonQuery, Encoding.UTF8, "application/json");
            HttpClient HttpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage lHttpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);
            string Content = await lHttpResponseMessage.Content.ReadAsStringAsync();
            JToken RsiShipStandaloneData = JObject.Parse(Content)["data"]!["store"]!["listing"]!["resources"]!;
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: RsiShipStandaloneData");
            return RsiShipStandaloneData;
        }


        private float NormalizePrice(string Price) {
            return Convert.ToSingle(Price) / 100;
        }

        private string NormalizeShipName(string ShipName)
        {
            return _shipNameDictionary.ContainsKey(ShipName) ? _shipNameDictionary[ShipName] : ShipName;
        }
        private string NormalizePackageName (string PackageName) {
            string NormalizePackageName = PackageName.Split(" Starter Pack")[0].Replace(" -", "");
            return _shipNameDictionary.ContainsKey(NormalizePackageName) ? _shipNameDictionary[NormalizePackageName] : NormalizePackageName;
        }

        private string NormalizeTaxDescription(string TaxDescription)
        {
            return TaxDescription.Replace("[", "").Replace("]", "").Replace("\"", "").Trim();
        }

        private async Task SaveRsiDataOnFile (List<Ship> rsiData) {
            DirectoryInfo Directory = new DirectoryInfo(_defaultPath);
            Directory.Create();
            string json = System.Text.Json.JsonSerializer.Serialize(rsiData);
            await File.WriteAllTextAsync(_defaultFilePath, json);
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Save RSI web data on a file in server");
        }
    }
}
