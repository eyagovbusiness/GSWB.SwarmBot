using MandrilAPI.Commands;
using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TGF.CA.Domain.Primitives.Result;

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
        public async Task<Result<bool>> GetUserExist(ulong aUserId, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new ExistDiscordUserQuery(aUserId), aCancellationToken);
        }

        [HttpGet("GetUserIsVerified")]
        public async Task<Result<bool>> GetUserIsVerified(ulong aUserId, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new IsUserVerifiedQuery(aUserId), aCancellationToken);
        }

        [HttpGet("GetUserCreationDate")]
        public async Task<Result<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new GetUserCreationDateQuery(aUserId), aCancellationToken);
        }

        [HttpGet("GetNumberOfOnlineUsers")]
        public async Task<Result<int>> GetNumberOfOnlineUsers(CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new GetNumberOfOnlineUsersQuery(), aCancellationToken);
        }

        [HttpGet("GetExistingCategoryId")]
        public async Task<Result<string>> GetExistingCategoryId(string aCategoryName, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new GetExistingCategoryIdQuery(aCategoryName), aCancellationToken);
        }


        #endregion

        #region Post

        [HttpPost("CreateRole")]
        public async Task<Result<string>> PostRole(string aRoleName, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new CreateRoleCommand(aRoleName), aCancellationToken);
        }

        [HttpPost("CreateCategoryFromTemplate")]
        public async Task<Result<string>> PostCreateCategoryFromTemplate(string aCategoryTemplateString, CancellationToken aCancellationToken)
        {
            var lCategoryChannelTemplate = JsonConvert.DeserializeObject<CategoryChannelTemplate>(aCategoryTemplateString);
            return await _mediator.Send(new CreateCategoryFromTemplateCommand(lCategoryChannelTemplate), aCancellationToken);
        }

        #endregion

        #region Put

        [HttpPut("AssignRoleToMember")]
        public async Task<Result> PostAssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new AssignRoleToMemberCommand(aRoleId, aFullDiscordHandle), aCancellationToken);
        }

        [HttpPut("RevokeRoleToMember")]
        public async Task<Result> PostRevokeRoleToMember(ulong aRoleId, string aFullDiscordHandle, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new RevokeRoleToMemberCommand(aRoleId, aFullDiscordHandle), aCancellationToken);
        }

        [HttpPut("AssignRoleToMemberList")]
        public async Task<Result> PutRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandleList, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new AssignRoleToMemberListCommand(aRoleId, aFullDiscordHandleList), aCancellationToken);
        }

        [HttpPut("RevokeRoleToMemberList")]
        public async Task<Result> PostRevokeRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandle, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new RevokeRoleToMemberListCommand(aRoleId, aFullDiscordHandle), aCancellationToken);
        }

        [HttpPut("AddMemberListToCategory")]
        public async Task<Result> PutAddMemberListToCategory(ulong aCategoryId, string[] aUserFullHandleList, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new AddMemberListToCategoryCommand(aCategoryId, aUserFullHandleList), aCancellationToken);
        }

        //[HttpPut("UpdateCategoryFromTemplate")]
        //public async Task<Result> UpdateCategoryFromTemplate(ulong aCategoryId, CategoryChannelTemplate aTemplate, CancellationToken aCancellationToken)
        //{
        //    return await _mediator.Send(new AddMemberListToCategoryCommand(aCategoryId, aTemplate), aCancellationToken);
        //}

        #endregion

        #region Delete

        [HttpDelete("DeleteCategory")]
        public async Task<Result> PostDeleteCategory(ulong aCategoryId, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new DeleteCategoryCommand(aCategoryId), aCancellationToken);
        }

        [HttpDelete("DeleteRole")]
        public async Task<Result> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken)
        {
            return await _mediator.Send(new DeleteRoleCommand(aRoleId), aCancellationToken);
        }

        #endregion

    }
}