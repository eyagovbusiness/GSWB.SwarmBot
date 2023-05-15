using System.Diagnostics.CodeAnalysis;

namespace MandrilBot.News
{
    public struct CommLinkNewsMessage
    {
        /// <summary>
        /// The HTML does not brin any DateTime, but how long ago the post was made in format ("30 minutes ago" or "2 hours ago"..).
        /// </summary>
        public string Date;
        /// <summary>
        /// Title of the post.
        /// </summary>
        public string Title;
        /// <summary>
        /// Official source link of the post.
        /// </summary>
        public string SourceLink;
    }

    /// <summary>
    /// Custom Equality comparer for CommLinkNewsMessage needed to ignore Date as it is changing every hour or minute, it depends see <see cref="CommLinkNewsMessage.Date"/>
    /// </summary>
    public class CommLinkNewsMessageComparer : IEqualityComparer<CommLinkNewsMessage>
    {
        public bool Equals(CommLinkNewsMessage x, CommLinkNewsMessage y)
            => x.Title == y.Title
               && x.SourceLink == y.SourceLink;

        public int GetHashCode([DisallowNull] CommLinkNewsMessage lObj)
            => HashCode.Combine(lObj.Title, lObj.SourceLink);
    
    }
}
