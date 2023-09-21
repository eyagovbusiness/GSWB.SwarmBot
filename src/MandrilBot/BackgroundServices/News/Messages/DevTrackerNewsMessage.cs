using System.Diagnostics.CodeAnalysis;

namespace MandrilBot.BackgroundServices.News.Messages
{
    internal struct DevTrackerNewsMessage
    {
        /// <summary>
        /// Developer who wrote the post.
        /// </summary>
        public string Author;
        /// <summary>
        /// Group of this news(i.e: Announcements, PatchNotes, General...).
        /// </summary>
        public string Group;
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
        /// <summary>
        /// Unique messageId identifying the dev message.
        /// </summary>
        public readonly string MessageId => SourceLink[(SourceLink.LastIndexOf('/') + 1)..];

    }

    /// <summary>
    /// Custom Equality comparer for DevTrackerNewsMessage needed to ignore Date as it is changing every hour or minute, it depends see <see cref="DevTrackerNewsMessage.Date"/>
    /// </summary>
    internal class DevTrackerNewsMessageComparer : IEqualityComparer<DevTrackerNewsMessage>
    {
        public bool Equals(DevTrackerNewsMessage x, DevTrackerNewsMessage y)
            => x.Author == y.Author
               && x.Title == y.Title
               && x.Description == y.Description
               && x.SourceLink == y.SourceLink
               && x.Group == y.Group;

        public int GetHashCode([DisallowNull] DevTrackerNewsMessage lObj)
            => HashCode.Combine(lObj.Author, lObj.Title, lObj.Description, lObj.SourceLink, lObj.Group);
    }
}
