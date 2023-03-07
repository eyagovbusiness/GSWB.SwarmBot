using DSharpPlus;
using MandrilBot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TGF.Common.Extensions;

namespace Mandril.API.IntegrationTests
{
    internal static class TestHelpers
    {
        internal static Random _random = new Random();
        internal static CategoryChannelTemplate GetCategoryChannelTemplateSample()
        {
            int lRandomSampleSize = _random.Next(2, 10);

            ChannelTemplate[] lChannelTemplateList = new ChannelTemplate[lRandomSampleSize];

            for(int i = 0; i < lRandomSampleSize; i++)
            {
                ChannelTemplate lChannelTemplate = new ChannelTemplate();
                lChannelTemplate.ChannelType = _random.Next(2) > 0 ? ChannelType.Text : ChannelType.Voice;
                lChannelTemplate.Position = i;
                lChannelTemplate.Name = "TestChannel" + i;
                lChannelTemplateList[i] = lChannelTemplate;
            }

            return new CategoryChannelTemplate() 
            { 
                Name = "TestingCategory",
                Position = 0,
                ChannelList= lChannelTemplateList

            };
        }
        internal static void RandomModifyTemplate()
        {
            //var lChannelTemplate = new ChannelTemplate()
            //{

            //};
            //return new CategoryChannelTemplate()
            //{

            //};
        }
    }
}
