using DSharpPlus;
using MandrilBot;

namespace Mandril.API.IntegrationTests
{
    internal static class TestHelpers
    {
        internal static Random _random = new Random();

        private static ChannelType GetRandomVoiceOrTextType()
        {
            return _random.Next(2) > 0 ? ChannelType.Text : ChannelType.Voice;
        }
        private static ChannelTemplate[] GetChannelTemplateSampleList()
        {
            int lRandomSampleSize = _random.Next(2, 10);

            ChannelTemplate[] lChannelTemplateList = new ChannelTemplate[lRandomSampleSize];

            for (int i = 0; i < lRandomSampleSize; i++)
            {
                ChannelTemplate lChannelTemplate = new ChannelTemplate();
                lChannelTemplate.ChannelType = GetRandomVoiceOrTextType();
                lChannelTemplate.Position = i;
                lChannelTemplate.Name = "TestChannel" + i;
                lChannelTemplateList[i] = lChannelTemplate;
            }
            return lChannelTemplateList;
        }
        internal static CategoryChannelTemplate GetCategoryChannelTemplateSample()
        {
            return new CategoryChannelTemplate()
            {
                Name = "TestingCategory",
                Position = 0,
                ChannelList = GetChannelTemplateSampleList()

            };
        }
        internal static void RandomModifyTemplate(this CategoryChannelTemplate aCategoryChannelTemplate)
        {
            var lModifyIndex = _random.Next(aCategoryChannelTemplate.ChannelList.Count());
            (aCategoryChannelTemplate.ChannelList as ChannelTemplate[])[lModifyIndex] = 
            new ChannelTemplate()
            {
                Name = "ModifiedAddedChannel",
                Position = aCategoryChannelTemplate.ChannelList.Max(channel => channel.Position) + 1,
                ChannelType = GetRandomVoiceOrTextType()
            };

        }
    }
}
