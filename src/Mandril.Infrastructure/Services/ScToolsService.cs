﻿using Mandril.Application;
using Mandril.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace Mandril.Infrastructure.Services
{
    internal class ScToolsService : IScToolsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _scToolsbaseUrlRsi;
        private readonly ILogger _logger;
        private readonly string _defaultFilePath = "/app/data/rsiData.json";

        public ScToolsService(IHttpClientFactory aHttpClientFactory, IConfiguration aConfiguration, ILogger<ScToolsService> aLogger)
        {
            _httpClientFactory = aHttpClientFactory;
            _logger = aLogger;
            _scToolsbaseUrlRsi = aConfiguration.GetValue<string>("ScToolsbaseUrlRsi")
                ?? throw new ArgumentNullException("Failed on fetching 'ScToolsbaseUrlRsi' from config, a value is required.");
        }

        public async Task<string> GetCookiesFromResponse(string url)
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };

            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                Uri uri = new Uri(url);
                return cookieContainer.GetCookies(uri)[0].Value;
            }
        }

        private async Task<IHttpResult<string>> GetRsiToken()
        {
            var rsiToken = await this.GetCookiesFromResponse(_scToolsbaseUrlRsi + "/pledge");
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: RsiToken");
            return Result.SuccessHttp(rsiToken);
        }

        private async Task<string> GetRsiAuthToken()
        {
            var rsiToken = (await this.GetRsiToken()).Value;
            var client = new HttpClient(new HttpClientHandler());
            client.DefaultRequestHeaders.Add("x-rsi-token", rsiToken);
            HttpResponseMessage response = await client.PostAsync(_scToolsbaseUrlRsi + "/api/account/v2/setAuthToken", null);
            var lJsonString = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Get Info from RSI web: RsiAuthToken");
            return JObject.Parse(lJsonString)["data"]!.ToString();
        }

        public async Task<IHttpResult<IEnumerable<Ship>>> GetRsiShipList()
        {
            var json = await File.ReadAllTextAsync(_defaultFilePath);
            var data = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Ship>>(json);
            return Result.SuccessHttp(data);
        }

        public async Task GetRsiData()
        {
            var RsiAuthToken = await this.GetRsiAuthToken();

            List<Ship> ListShip = new List<Ship>();

            JToken ShipMatrixData = await this.GetRsiShipMatrixData(RsiAuthToken);
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
                        Link = dataShip["url"]!.ToString()
                    });
            });

            JToken ShipCcuData = await this.GetRsiShipCcuData(RsiAuthToken);

            ShipCcuData["from"]!["ships"]!.ForEach(dataShip =>
            {
                var index = ListShip.FindIndex(Ship => Ship.Id == dataShip["id"]!.ToString());
                ListShip[index].Price = (Convert.ToSingle(dataShip["msrp"]) / 100);
            });

            ShipCcuData["to"]!["ships"]!.ForEach(dataShip =>
            {
                var index = ListShip.FindIndex(Ship => Ship.Id == dataShip["id"]!.ToString());

                List<ShipCcu> CcuList = new List<ShipCcu>();
                foreach (var CcuData in dataShip["skus"]!)
                {
                    string CcuId = CcuData["id"]!.ToString();
                    string CcuName = CcuData["title"]!.ToString();
                    float CcuPrice = Convert.ToSingle(CcuData["price"]) / 100;
                    CcuList.Add(new ShipCcu() { Id = CcuId, Name = CcuName, Price = CcuPrice });
                }
                ListShip[index].CcuList = CcuList;
            });


            await this.SaveRsiDataOnFile(ListShip);
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

        private async Task SaveRsiDataOnFile (List<Ship> rsiData) {
            var json = System.Text.Json.JsonSerializer.Serialize(rsiData);
            FileInfo file = new FileInfo(_defaultFilePath);
            file.Directory.Create();
            await File.WriteAllTextAsync(_defaultFilePath, json);
            _logger.LogInformation("[SC_TOOLS_SERVICES] [SUCCESS] Save RSI web data on a file in server");
        }
    }
}
