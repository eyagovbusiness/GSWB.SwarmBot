using System.Diagnostics.CodeAnalysis;

namespace SwarmBot.BackgroundServices.News.Messages
{
    internal struct YouTubeNewsMessage
    {
        /// <summary>
        /// Title of the post.
        /// </summary>
        public string Title;
        /// <summary>
        /// Description of the post.
        /// </summary>
        public string Description;
        /// <summary>
        /// Link to the new video.
        /// </summary>
        public string VideoLink;
        /// <summary>
        /// Official image link of the video image.
        /// </summary>
        public string ThumbnailLink;
    }

    /// <summary>
    /// Custom Equality comparer for YouTubeNewsMessage
    /// </summary>
    internal class YouTubeNewsMessageComparer : IEqualityComparer<YouTubeNewsMessage>
    {
        public bool Equals(YouTubeNewsMessage x, YouTubeNewsMessage y)
            => x.Title == y.Title
               && x.VideoLink == y.VideoLink;

        public int GetHashCode([DisallowNull] YouTubeNewsMessage lObj)
            => HashCode.Combine(lObj.Title, lObj.VideoLink);

    }
}
