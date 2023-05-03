using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TGF.Common.ROP.Errors;
using TGF.Common.ROP.Result;

namespace Mandril.API.IntegrationTests
{
    /// <summary>
    /// Common methods shared between all CRUD integration tests.
    /// </summary>
    public static class TestCommon
    {
        public static readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:7002") };
        public const string _jsonMediaType = "application/json";
        public static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<T> AssertResponseWithContentAsync<T>(
            HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode,
            Func<T, bool> aResponseValidationFunc)
        {
            AssertCommonResponseParts(response, expectedStatusCode);
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo(_jsonMediaType));

            var lJsonString = await response.Content.ReadAsStringAsync();
            var lContent = JsonConvert.DeserializeObject<T>(lJsonString);

            Assert.That(lContent != null && aResponseValidationFunc.Invoke(lContent));

#pragma warning disable CS8603 // Possible null reference return.
            return lContent;
#pragma warning restore CS8603 // Possible null reference return.
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
    [JsonObject]
    public class Result<T>
    { 
        [JsonPropertyName("Value")]
        public T Value { get; set; }

        public Result(T aValue, bool aIsuccess, ImmutableArray<Error> aErrorList)
        {
            Value = aValue;
            IsSuccess = aIsuccess;
            ErrorList = aErrorList;
        }

        [JsonPropertyName("IsSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("IsSuccess")]
        public ImmutableArray<Error> ErrorList { get; set; }



    }
    public class Error : IError
    {
        public string Code { get; }
        public string Message { get; }
        public Error(string aCode, string aMessage)
        {
            Code = aCode;
            Message = aMessage;
        }
    }

}
