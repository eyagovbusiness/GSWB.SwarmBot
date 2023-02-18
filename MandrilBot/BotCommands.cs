using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandrilBot
{
    internal class BotCommands : BaseCommandModule
    {
        [Command("start-service")]
        public async Task StartServiceBotCommand(CommandContext aCommandContext)
        {
            /*var lEventCategoryId = aCommandContext.Channel.Parent.Id;*/ //With this we can go to the web DB and read the event associated with this category.
                                                                          //Next step would be to read to which channel the user who sent the command is assignes in this event and move him to that channel.
            try
            {
                await aCommandContext.Member
                                     .PlaceInAsync(aCommandContext.Channel.Parent.Children
                                        .FirstOrDefault(x => x.Type == DSharpPlus.ChannelType.Voice))
                                     .ConfigureAwait(false);
            }
            catch(BadRequestException) 
            { 
                await aCommandContext.Channel.SendMessageAsync("Please, be connected to any voice channel in this server before reporting for service :)"); 
            }

        }
    }
}
