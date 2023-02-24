using MandrilAPI.Commands;
using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MandrilController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MandrilController(IMediator aMediator)
        {
            _mediator = aMediator;
        }

        #region Get

        [HttpGet("GetUserExist")]
        public async Task<Result<bool>> GetUserExist(ulong aUserId)
        {
            return await _mediator.Send(new ExistDiscordUserQuery(aUserId));
        }

        [HttpGet("GetUserIsVerified")]
        public async Task<Result<bool>> GetUserIsVerified(ulong aUserId)
        {
            return await _mediator.Send(new IsUserVerifiedQuery(aUserId));
        }

        [HttpGet("GetUserCreationDate")]
        public async Task<Result<DateTimeOffset>> GetUserCreationDate(ulong aUserId)
        {
            return await _mediator.Send(new GetUserCreationDateQuery(aUserId));
        }

        [HttpGet("GetNumberOfOnlineUsers")]
        public async Task<Result<int>> GetNumberOfOnlineUsers()
        {
            return await _mediator.Send(new GetNumberOfOnlineUsersQuery());
        }


        #endregion

        #region Post

        [HttpPost("CreateRole")]
        public async Task<Result<string>> PostRole(string aRoleName)
        {
            return await _mediator.Send(new CreateRoleCommand(aRoleName));
        }

        [HttpPost("CreateCategoryFromTemplate")]
        public async Task<Result<string>> PostCreateCategoryFromTemplate(string aCategoryTemplateString)
        {
            var lCategoryChannelTemplate = JsonConvert.DeserializeObject<CategoryChannelTemplate>(aCategoryTemplateString);
            return await _mediator.Send(new CreateCategoryFromTemplateCommand(lCategoryChannelTemplate));
        }

        #endregion

        #region Put

        [HttpPut("AssignRoleToUser")]
        public async Task<Result> PostAssignRoleToUser(ulong aRoleId, string aFullDiscordHandle)
        {
            return await _mediator.Send(new AssignRoleToUserCommand(aRoleId, aFullDiscordHandle));
        }

        [HttpPut("RevokeRoleToUser")]
        public async Task<Result> PostRevokeRoleToUser(ulong aRoleId, string aFullDiscordHandle)
        {
            return await _mediator.Send(new RevokeRoleToUserCommand(aRoleId, aFullDiscordHandle));
        }

        [HttpPut("AssignRoleToUserList")]
        public async Task<Result> PostRoleToUserList(ulong aRoleId, string[] aFullDiscordHandleList)
        {
            return await _mediator.Send(new AssignRoleToUserListCommand(aRoleId, aFullDiscordHandleList));
        }

        [HttpPut("RevokeRoleToMemberList")]
        public async Task<Result> PostRevokeRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandle)
        {
            return await _mediator.Send(new RevokeRoleToMemberListCommand(aRoleId, aFullDiscordHandle));
        }

        [HttpPut("AddUserListToCategory")]
        public async Task<Result> PostAddUserListToCategory(ulong aCategoryId, string[] aUserFullHandleList)
        {
            return await _mediator.Send(new AddUserListToCategoryCommand(aCategoryId, aUserFullHandleList));
        }

        #endregion

        #region Delete

        [HttpDelete("DeleteCategory")]
        public async Task<Result> PostDeleteCategory(ulong aCategoryId)
        {
            return await _mediator.Send(new DeleteCategoryCommand(aCategoryId));
        }

        #endregion

    }
}