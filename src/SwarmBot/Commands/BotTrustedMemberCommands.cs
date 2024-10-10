using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGF.Common.Extensions;

namespace SwarmBot.Commands
{
    /// <summary>
    /// Class with definition of the Discord bot commands that can be used to interact with the bot from Discord only by members with the required trusted member role assigned.
    /// </summary>
    internal class BotTrustedMemberCommands : BotAuthorizedCommands
    {
        public BotTrustedMemberCommands
            (IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration)
                : base(aServiceScopeFactory, aConfiguration, "TrustedMemberRoleId")
        {
        }

        [Command("moveFromRole")]
        [Description("Move to the target voice channel all members connected to the same voice channel as the command caller with the given role or higher.")]
        public async Task MoveFromRole(CommandContext aContext,
            [Description("Channel to move members to.")] DiscordChannel aTargetChannel,
            [Description("Role to start filtering members by hierarchy.")] DiscordRole aBaseRole
)
        {
            var lMemberListToMove = await Move_GetMemberList(aContext, aTargetChannel, aBaseRole);

            //Filter members who are in the same channel as the command caller
            var lCallerChannel = aContext.Member?.VoiceState?.Channel;
            if (lCallerChannel == null)
            {
                await aContext.RespondAsync("You must be in a voice channel to use the non-global move!");
                return;
            }
            lMemberListToMove = lMemberListToMove.Where(m => m.VoiceState?.Channel == lCallerChannel).ToList();

            await Move_Execute(aContext, aTargetChannel, lMemberListToMove);
        }

        [Command("moveRoleList")]
        [Description("Move to the target voice channel all members connected to the same voice channel as the command caller with at least one of the provided roles assigned.")]
        public async Task MoveRoleList(CommandContext aContext,
            [Description("Channel to move members to.")] DiscordChannel aTargetChannel,
            [Description("List of Roles to determine which members to move.")] params DiscordRole[] aRoleList)
        {
            var lMemberListToMove = await Move_GetMemberList(aContext, aTargetChannel, default!, aRoleList);

            //Filter members who are in the same channel as the command caller
            var lCallerChannel = aContext.Member?.VoiceState?.Channel;
            if (lCallerChannel == null)
            {
                await aContext.RespondAsync("You must be in a voice channel to use the non-global move!");
                return;
            }
            lMemberListToMove = lMemberListToMove.Where(m => m.VoiceState?.Channel == lCallerChannel).ToList();

            await Move_Execute(aContext, aTargetChannel, lMemberListToMove);
        }

        [Command("moveFromRoleGlobal")]
        [Description("Move to the target voice channel all members connected to any voice channel with the given role or higher.")]
        public async Task MoveFromRoleGlobal(CommandContext aContext,
            [Description("Channel to move members to.")] DiscordChannel aTargetChannel,
            [Description("Role to start filtering members by hierarchy.")] DiscordRole aBaseRole)
        {
            var lMemberListToMove = await Move_GetMemberList(aContext, aTargetChannel, aBaseRole);
            await Move_Execute(aContext, aTargetChannel, lMemberListToMove);
        }

        [Command("moveRoleListGlobal")]
        [Description("Move to the target voice channel all members connected to any voice channel with at least one of the provided roles assigned.")]
        public async Task moveRoleListGlobal(CommandContext aContext,
            [Description("Channel to move members to.")] DiscordChannel aTargetChannel,
            [Description("List of Roles to determine which members to move.")] params DiscordRole[] aRoleList)
        {
            var lMemberListToMove = await Move_GetMemberList(aContext, aTargetChannel, default!, aRoleList);
            await Move_Execute(aContext, aTargetChannel, lMemberListToMove);
        }

        #region Private

        /// <summary>
        /// Returns checks if the the member who executed the command has permissions and in that case get a list of members to be moved based on a move command context and parameters, otherwise an empty list is returned.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <description>Set aRoleOrHigher when the move command takes members with the given tole or higher and ignore aRoleList.</description>
        /// </item>
        /// <item>
        /// <description>Set as many DiscordRole in aRoleList as roles you want to move to the target channel, any user with any of the roles in the list assigned will be moved.</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>list of members to be moved to the target channel.</returns>
        private async Task<IEnumerable<DiscordMember>> Move_GetMemberList(CommandContext aContext, DiscordChannel aTargetChannel, DiscordRole aRoleOrHigher = default!, params DiscordRole[] aRoleList)
        {
            if (!await IsMemberAuthorized(aContext.Member!))
                return [];

            if (aTargetChannel.Type != ChannelType.Voice)
            {
                await aContext.RespondAsync("Please specify a valid voice channel!");
                return [];
            }

            var lAllMemberList = await aContext.Guild.GetAllMembersAsync();

            if (aRoleOrHigher != null)
            {
                int lBaseRolePosition = aRoleOrHigher.Position;
                return lAllMemberList.Where(m => m.Roles.Any(r => r.Position >= lBaseRolePosition) && m.VoiceState?.Channel != null).ToList();
            }
            if (aRoleList.Length > 0)
            {
                var lRoleIdList = aRoleList.Select(role => role.Id).ToList();
                return lAllMemberList
                .Where(member =>
                    member.VoiceState?.Channel != null
                    && lRoleIdList.Intersect(
                        member.Roles.Select(role => role.Id))
                    .Any()
                ).ToList();
            }
            return Array.Empty<DiscordMember>();
        }

        /// <summary>
        /// Moves each member in the list to the target channel. If some error happens during the process send an error message through the command context channel.
        /// </summary>
        private static async Task Move_Execute(CommandContext aContext, DiscordChannel aTargetChannel, IEnumerable<DiscordMember> aMemberListToMove)
        {
            await aMemberListToMove.ParallelForEachAsync(3, async (member) =>
            {
                try
                {
                    await member.ModifyAsync(m => m.VoiceChannel = aTargetChannel);
                }
                catch (Exception lEx)
                {
                    await aContext.RespondAsync($"Failed to move {member.Username}: {lEx.Message}");
                }
            });


            await aContext.RespondAsync($"Members were moved to `{aTargetChannel.Name}`.");
        }

        #endregion

    }
}
