using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class RevokeRoleToMemberCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string FullDiscordIdentifier { get; set; }

        public RevokeRoleToMemberCommand(ulong aRoleId, string aFullDiscordIdentifier)
        {
            RoleId = aRoleId;
            FullDiscordIdentifier = aFullDiscordIdentifier;

        }

    }
}
