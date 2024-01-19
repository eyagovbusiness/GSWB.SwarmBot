
namespace SwarmBot.Application.DTOs
{
    /// <summary>
    /// Represents a discord channel template with all the required fields in order to create this channel in Discord.
    /// </summary>
    /// <param name="Name">Name of the new category.</param>
    /// <param name="Position">Position where the category will be placed in the list of channels avaliable in the server.</param>
    /// <param name="ChannelType">Type of channel(voice, text, etc.)</param>
    public record ChannelTemplateDTO(string Name, int? Position, DSharpPlus.ChannelType ChannelType);

}
