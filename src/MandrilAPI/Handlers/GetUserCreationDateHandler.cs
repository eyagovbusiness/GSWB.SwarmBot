using MandrilAPI.Queries;
using MandrilBot;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetUserCreationDateHandler : IRequestHandler<GetUserCreationDateQuery, IResult<DateTimeOffset>>
    {
        private readonly IUsersController _usersController;
        public GetUserCreationDateHandler(IUsersController aUsersController)
            => _usersController = aUsersController;
        
        public async Task<IResult<DateTimeOffset>> Handle(GetUserCreationDateQuery aRequest, CancellationToken aCancellationToken)
            => await _usersController.GetUserCreationDate(aRequest.UserId, aCancellationToken);

    }
}

