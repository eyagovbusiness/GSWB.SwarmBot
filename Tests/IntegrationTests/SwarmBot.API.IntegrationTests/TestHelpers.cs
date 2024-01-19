using DSharpPlus;
using SwarmBot.Application.DTOs;

namespace SwarmBot.API.IntegrationTests
{
    internal static class TestHelpers
    {
        internal static Random _random = new();

        private static ChannelType GetRandomVoiceOrTextType()
        {
            return _random.Next(2) > 0 ? ChannelType.Text : ChannelType.Voice;
        }
        private static ChannelTemplateDTO[] GetChannelTemplateSampleList()
        {
            int lRandomSampleSize = _random.Next(2, 10);

            ChannelTemplateDTO[] lChannelTemplateList = new ChannelTemplateDTO[lRandomSampleSize];

            for (int i = 0; i < lRandomSampleSize; i++)
            {
                var lChannelTemplate = new ChannelTemplateDTO("TestChannel" + i, i, GetRandomVoiceOrTextType());
                lChannelTemplateList[i] = lChannelTemplate;
            }
            return lChannelTemplateList;
        }
        internal static CategoryChannelTemplateDTO GetCategoryChannelTemplateSample()
            => new("TestingCategory", 0, GetChannelTemplateSampleList());
        internal static void RandomModifyTemplate(this CategoryChannelTemplateDTO aCategoryChannelTemplate)
        {
            var lModifyIndex = _random.Next(aCategoryChannelTemplate.ChannelList!.Length);
            aCategoryChannelTemplate.ChannelList![lModifyIndex] =
            new ChannelTemplateDTO("ModifiedAddedChannel", aCategoryChannelTemplate.ChannelList.Max(channel => channel.Position) + 1, GetRandomVoiceOrTextType());
        }
    }
}
