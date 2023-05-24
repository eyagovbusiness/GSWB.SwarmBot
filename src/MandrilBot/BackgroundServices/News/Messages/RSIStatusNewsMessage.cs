using System.Diagnostics.CodeAnalysis;

namespace MandrilBot.BackgroundServices.News.Messages
{
    internal struct RSIStatusNewsMessage
    {
        public string IncidentStatus;
        public string IncidentTitle;
        public string AffectedServices;
        public string ServicesStatus;
        public string IncidentCreationDate;
        public string IncidentDescription;

    }

    /// <summary>
    /// Custom Equality comparer for CommLinkNewsMessage needed to ignore Date as it is changing every hour or minute, it depends see <see cref="CommLinkNewsMessage.Date"/>
    /// </summary>
    internal class RSIStatusNewsMessageComparer : IEqualityComparer<RSIStatusNewsMessage>
    {
        public bool Equals(RSIStatusNewsMessage x, RSIStatusNewsMessage y)
            => x.IncidentStatus == y.IncidentStatus
            && x.IncidentTitle == y.IncidentTitle
            && x.ServicesStatus == y.ServicesStatus
            && x.IncidentDescription == y.IncidentDescription;

        public int GetHashCode([DisallowNull] RSIStatusNewsMessage lObj)
            => HashCode.Combine(lObj.IncidentStatus, lObj.IncidentTitle, lObj.ServicesStatus, lObj.IncidentDescription);

    }
}
