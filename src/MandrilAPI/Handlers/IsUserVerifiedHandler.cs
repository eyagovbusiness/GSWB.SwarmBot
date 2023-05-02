using MandrilAPI.Queries;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class IsUserVerifiedHandler : IRequestHandler<IsUserVerifiedQuery, IResult<bool>>
    {
        private readonly IUsersController _usersController;
        public IsUserVerifiedHandler(IUsersController aUsersController)
            => _usersController = aUsersController;

        public async Task<IResult<bool>> Handle(IsUserVerifiedQuery aRequest, CancellationToken aCancellationToken)
            => await _usersController.IsUserVerified(aRequest.UserId, aCancellationToken);

    }
}

