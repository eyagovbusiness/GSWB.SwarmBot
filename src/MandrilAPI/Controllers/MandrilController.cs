using MandrilAPI.Commands;
using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

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
        [ProducesResponseType(typeof(IResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IResult<bool>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserExist(ulong aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new ExistDiscordUserQuery(aUserId), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetUserIsVerified")]
        public async Task<IActionResult> GetUserIsVerified(ulong aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new IsUserVerifiedQuery(aUserId), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetUserCreationDate")]
        public async Task<IActionResult> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new GetUserCreationDateQuery(aUserId), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetNumberOfOnlineMembers")]
        public async Task<IActionResult> GetNumberOfOnlineMembers(CancellationToken aCancellationToken)
            => await _mediator.Send(new GetNumberOfOnlineMembersQuery(), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetExistingCategoryId")]
        public async Task<IActionResult> GetExistingCategoryId(string aCategoryName, CancellationToken aCancellationToken) 
            => await _mediator.Send(new GetExistingCategoryIdQuery(aCategoryName), aCancellationToken)
                .ToActionResult();


        #endregion

        #region Post

        [HttpPost("CreateRolee")]
        public async Task<IActionResult> PostRole(string aRoleName, CancellationToken aCancellationToken)
            => await _mediator.Send(new CreateRoleCommand(aRoleName), aCancellationToken)
                .ToActionResult();


        [HttpPost("CreateCategoryFromTemplate")]
        public async Task<IActionResult> PostCreateCategoryFromTemplate([FromBody] CategoryChannelTemplate aTemplate, CancellationToken aCancellationToken)
            => await _mediator.Send(new CreateCategoryFromTemplateCommand(aTemplate), aCancellationToken)
                .ToActionResult();


        #endregion

        #region Put

        [HttpPut("AssignRoleToMember")]
        public async Task<IActionResult> PostAssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, CancellationToken aCancellationToken) 
            => await _mediator.Send(new AssignRoleToMemberCommand(aRoleId, aFullDiscordHandle), aCancellationToken)
                .ToActionResult();


        [HttpPut("RevokeRoleToMember")]
        public async Task<IActionResult> PostRevokeRoleToMember(ulong aRoleId, string aFullDiscordHandle, CancellationToken aCancellationToken)
            => await _mediator.Send(new RevokeRoleToMemberCommand(aRoleId, aFullDiscordHandle), aCancellationToken)
                .ToActionResult();

        [HttpPut("AssignRoleToMemberList")]
        public async Task<IActionResult> PutRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandleList, CancellationToken aCancellationToken)
            => await _mediator.Send(new AssignRoleToMemberListCommand(aRoleId, aFullDiscordHandleList), aCancellationToken)
                .ToActionResult();

        [HttpPut("RevokeRoleToMemberList")]
        public async Task<IActionResult> PostRevokeRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandle, CancellationToken aCancellationToken)
            => await _mediator.Send(new RevokeRoleToMemberListCommand(aRoleId, aFullDiscordHandle), aCancellationToken)
                .ToActionResult();


        [HttpPut("AddMemberListToCategory")]
        public async Task<IActionResult> PutAddMemberListToCategory(ulong aCategoryId, string[] aUserFullHandleList, CancellationToken aCancellationToken)
            => await _mediator.Send(new AddMemberListToCategoryCommand(aCategoryId, aUserFullHandleList), aCancellationToken)
                .ToActionResult();


        [HttpPut("UpdateCategoryFromTemplate")]
        public async Task<IActionResult> UpdateCategoryFromTemplate(ulong aCategoryId, CategoryChannelTemplate aTemplate, CancellationToken aCancellationToken)
            => await _mediator.Send(new UpdateCategoryFromTemplateCommand(aCategoryId, aTemplate), aCancellationToken)
                .ToActionResult();


        #endregion

        #region Delete

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> PostDeleteCategory(ulong aCategoryId, CancellationToken aCancellationToken)
            => await _mediator.Send(new DeleteCategoryCommand(aCategoryId), aCancellationToken)
                .ToActionResult();


        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken)
            => await _mediator.Send(new DeleteRoleCommand(aRoleId), aCancellationToken)
                .ToActionResult();


        #endregion

    }
}