namespace SwarmBot.Application.DTOs
{
    //TO-DO: GSWB-27, User.DisplayName is borken in DSharpPlus. Worarrounded in IMemberService.AddNewMember using DiscordCookieUserInfo.GivenName which will be removed when after a DSharpPlus fix is released.
    public record DiscordProfileDTO(string UserDisplayName, string AvatarUrl);

}
