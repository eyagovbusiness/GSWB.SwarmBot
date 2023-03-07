using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using TGF.CA.Domain.Primitives;

namespace Mandril.API.IntegrationTests
{
    /// <summary>
    /// Common methods shared between all CRUD integration tests.
    /// </summary>
    public static class TestCommon
    {
        public static readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:7001") };
        public const string _jsonMediaType = "application/json";
        public static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<T> AssertResponseWithContentAsync<T>(
            HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode,
            Func<T, bool> aResponseValidationFunc)
        {
            AssertCommonResponseParts(response, expectedStatusCode);
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo(_jsonMediaType));

            var lContent = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(
                await response.Content.ReadAsStreamAsync());

            Assert.That(aResponseValidationFunc.Invoke(lContent));

            return lContent;
        }


        public static void AssertCommonResponseParts(
            HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode)
        {
            Assert.That(expectedStatusCode, Is.EqualTo(response.StatusCode));
        }

        /// <summary>
        /// Used for building HTTP content from an object serializing it and instanciating a new <see cref="StringContent"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static StringContent GetJsonStringContent<T>(T model)
            => new(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);
    }
    public struct ResultStruct<T>
    {
        [JsonProperty("value")]
        public T value { get; set; }
        [JsonProperty("isSuccess")]
        public bool isSuccess { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public struct ResultStruct
    {
        [JsonProperty("isSuccess")]
        public bool isSuccess { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
}
