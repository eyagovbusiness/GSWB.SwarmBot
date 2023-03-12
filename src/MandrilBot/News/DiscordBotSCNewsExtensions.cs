using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandrilBot.News
{
    /// <summary>
    /// Class to provid needed common extensions to support different news alerts in Dicord about StarCitizen.
    /// </summary>
    internal static class DiscordBotSCNewsExtensions
    {
        internal static readonly string _baseAddress = "https://robertsspaceindustries.com";
        internal static readonly HttpClient _httpClient = new() { BaseAddress = new Uri(_baseAddress) };

        /// <summary>
        /// Returns an instance of <see cref="IHtmlDocument"/> with the HTML documment of the requiered source.
        /// </summary>
        /// <param name="aPath"></param>
        /// <returns>Returns an instance of <see cref="IHtmlDocument"/></returns>
        internal static async Task<IHtmlDocument> GetHTMLAsync(string aPath)
        {
            var lResponse = await _httpClient.GetAsync("community/devtracker");
            var lStringResponse = await lResponse.Content.ReadAsStringAsync();

            var lParser = new HtmlParser();
            return await lParser.ParseDocumentAsync(lStringResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aAuthorUrl"></param>
        /// <returns></returns>
        internal static async Task<string> GetCitizenImageLink(string aAuthorUrl)
        {
            HttpClient _httpClient = new() { BaseAddress = new Uri("https://robertsspaceindustries.com") };
            var lResponse = await _httpClient.GetAsync(aAuthorUrl);
            var lStringResponse = await lResponse.Content.ReadAsStringAsync();

            var lParser = new HtmlParser();
            var lHTMLdocument = lParser.ParseDocument(lStringResponse);

            var lElementList = lHTMLdocument.QuerySelector("div.thumb");
            var lRes = (lElementList.Children.First() as IHtmlImageElement).Source.Replace("about://", string.Empty);
            return lRes;
        }

    }
}
