
namespace SwarmBot.Application.DTOs
{
    /// <summary>
    /// Represents a discord channel category template with all the required fields in order to create this category in Discord.
    /// </summary>
    /// <param name="Name">Name of the new category.</param>
    /// <param name="Position">Position where the category will be placed in the list of channels avaliable in the server.</param>
    /// <param name="ChannelList">List of new channels that will be created inside this new category.</param>
    public record CategoryChannelTemplateDTO(string Name, int? Position, ChannelTemplateDTO[]? ChannelList);

}
