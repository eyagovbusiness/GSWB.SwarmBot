using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Exceptions;

namespace MandrilBot.Commands
{
    /// <summary>
    /// Class with definition of the Discord bot commands that can be used to interact with the bot from Discord by anyone in the server.
    /// </summary>
    internal class BotOpenedCommands : BaseCommandModule
    {
        public BotOpenedCommands()
        {
        }



        /// <summary>
        /// WIP
        /// </summary>
        /// <param name="aCommandContext"></param>
        /// <returns></returns>
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
                throw new NotImplementedException();
            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Please, be connected to any voice channel in this server before reporting for service :)");
            }

        }
    }
}
