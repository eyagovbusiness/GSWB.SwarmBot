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
    internal static class DiscordBotNewsExtensions
    {
        /// <summary>
        /// Returns an instance of <see cref="IHtmlDocument"/> with the HTML documment of the requiered source.
        /// </summary>
        /// <param name="aResourcePath">Relative path of the requiered resource.</param>
        /// <returns>Returns an instance of <see cref="IHtmlDocument"/>.</returns>
        internal static async Task<IHtmlDocument> GetHTMLAsync(HttpClient aHttpClient, string aResourcePath)
        {
            var lResponse = await aHttpClient.GetAsync(aResourcePath);
            var lStringResponse = await lResponse.Content.ReadAsStringAsync();

            var lParser = new HtmlParser();
            return await lParser.ParseDocumentAsync(lStringResponse);
        }

        /// <summary>
        /// Gets an <see cref="string"/> representing the <see cref="Uri"/> related to the official image of the given Author.
        /// </summary>
        /// <param name="aAuthorPath">Relative path of the resource of a given author.</param>
        /// <returns><see cref="string"/> representing the <see cref="Uri"/> related to the official image of the given Author.</returns>
        internal static async Task<string> GetCitizenImageLink(HttpClient aHttpClient, string aAuthorPath)
        {
            var lResponse = await aHttpClient.GetAsync(aAuthorPath);
            var lStringResponse = await lResponse.Content.ReadAsStringAsync();

            var lParser = new HtmlParser();
            var lHTMLdocument = lParser.ParseDocument(lStringResponse);

            var lElementList = lHTMLdocument.QuerySelector("div.thumb");
            var lRes = (lElementList.Children.First() as IHtmlImageElement).Source.Replace("about://", string.Empty);
            return lRes;
        }

    }
}
