using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandrilBot.News
{
    public struct DevTrackerNewsMessage
    {
        /// <summary>
        /// Developer who wrote the post.
        /// </summary>
        public string Author;
        /// <summary>
        /// The HTML does not brin any DateTime, but how long ago the post was made in format ("30 minutes ago" or "2 hours ago"..).
        /// </summary>
        public string Date;
        /// <summary>
        /// Title of the post.
        /// </summary>
        public string Title;
        /// <summary>
        /// Actual content of the post.
        /// </summary>
        public string Description;
        /// <summary>
        /// Official source link of the post.
        /// </summary>
        public string SourceLink;

    }
    /// <summary>
    /// Custom Equality comparer for DevTrackerNewsMessage needed to ignore Date as it is changing every hour or minute, it depends see <see cref="DevTrackerNewsMessage.Date"/>
    /// </summary>
    public class DevTrackerNewsMessageComparer : IEqualityComparer<DevTrackerNewsMessage>
    {
        public bool Equals(DevTrackerNewsMessage x, DevTrackerNewsMessage y)
        {
            return x.Author == y.Author
                && x.Title == y.Title
                && x.Description == y.Description
                && x.SourceLink == y.SourceLink;
        }

        public int GetHashCode([DisallowNull] DevTrackerNewsMessage lObj)
        {
            return HashCode.Combine(lObj.Author, lObj.Title, lObj.Description, lObj.SourceLink);
        }
    }
}
