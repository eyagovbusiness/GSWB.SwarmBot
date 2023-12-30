﻿using Mandril.Application;
using Mandril.Domain.ValueObjects;
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
            var rsiToken = await this.GetCookiesFromResponse("https://robertsspaceindustries.com/pledge");
            return Result.SuccessHttp(rsiToken);
        }

        private async Task<string> GetRsiAuthToken()
        {
            var rsiToken = (await this.GetRsiToken()).Value;
            var client = new HttpClient(new HttpClientHandler());
            client.DefaultRequestHeaders.Add("x-rsi-token", rsiToken);
            HttpResponseMessage response = await client.PostAsync("https://robertsspaceindustries.com/api/account/v2/setAuthToken", null);
            var lJsonString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(lJsonString)["data"]!.ToString();
        }

        public async Task<IHttpResult<IEnumerable<Ship>>> GetRsiShipList()
        {
            var RsiAuthToken = await this.GetRsiAuthToken();

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            cookies.Add(new Uri("https://robertsspaceindustries.com"), new Cookie("Rsi-Account-Auth", RsiAuthToken));
            handler.CookieContainer = cookies;
            handler.UseCookies = true;
            var client = new HttpClient(handler);
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
            var content = new StringContent(jsonQuery, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://robertsspaceindustries.com/pledge-store/api/upgrade/graphql", content);
            string responseContent = await response.Content.ReadAsStringAsync();

            var ListShip = new List<Ship>();
            
            var RawDataShips = JObject.Parse(responseContent)["data"]!["from"]!["ships"];
            RawDataShips.ForEach(dataShip =>
            {
                string Id = dataShip["id"]!.ToString();
                string Name = dataShip["name"]!.ToString();
                float Price = Convert.ToSingle(dataShip["msrp"]) / 100;
                bool IsConcept = dataShip["flyableStatus"]!.ToString() == "Concept";
                ListShip.Add(
                    new Ship() {
                        Id = Id,
                        Name = Name,
                        Price = Price,
                        IsConcept = IsConcept,
                        Images = new ShipImages() {
                            Small = dataShip["medias"]!["productThumbMediumAndSmall"]!.ToString(),
                            Medium = dataShip["medias"]!["slideShow"]!.ToString(),
                        },
                        Manufacturer = new ShipManufacturer()
                        {
                            Id = dataShip["manufacturer"]!["id"]!.ToString(),
                            Name = dataShip["manufacturer"]!["name"]!.ToString()
                        },
                        Focus = dataShip["focus"]!.ToString(),
                        Type = dataShip["type"]!.ToString(),
                        Link = dataShip["link"]!.ToString()
                    });
            });

            RawDataShips = JObject.Parse(responseContent)["data"]!["to"]!["ships"];
            RawDataShips!.ForEach(dataShip =>
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
                ListShip[index].CcuList =CcuList;
            });

            return Result.SuccessHttp(ListShip as IEnumerable<Ship>);
        }
    }
}
