using MandrilAPI.Queries;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class ExistDiscordUserHandler : IRequestHandler<ExistDiscordUserQuery, IResult<bool>>
    {
        private readonly IUsersController _usersController;
        public ExistDiscordUserHandler(IUsersController aUsersController)
            => _usersController = aUsersController;

        public async Task<IResult<bool>> Handle(ExistDiscordUserQuery aRequest, CancellationToken aCancellationToken)
            => await _usersController.ExistUser(aRequest.UserId, aCancellationToken);

    }
}

