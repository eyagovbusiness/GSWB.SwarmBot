using DSharpPlus.Entities;
using MandrilAPI.Commands;
using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MandrilController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MandrilController(IMediator aMediator)
            => _mediator = aMediator;

        #region Get

        [HttpGet("GetUserExist")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserExist(ulong aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new ExistDiscordUserQuery(aUserId), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetUserIsVerified")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserIsVerified(ulong aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new IsUserVerifiedQuery(aUserId), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetUserCreationDate")]
        [ProducesResponseType(typeof(DateTimeOffset), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(DateTimeOffset), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(DateTimeOffset), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(DateTimeOffset), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new GetUserCreationDateQuery(aUserId), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetNumberOfOnlineMembers")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNumberOfOnlineMembers(CancellationToken aCancellationToken)
            => await _mediator.Send(new GetNumberOfOnlineMembersQuery(), aCancellationToken)
                .ToActionResult();


        [HttpGet("GetExistingCategoryId")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetExistingCategoryId(string aCategoryName, CancellationToken aCancellationToken)
            => await _mediator.Send(new GetExistingCategoryIdQuery(aCategoryName), aCancellationToken)
                .ToActionResult();

        [HttpGet("GetMemberHighestRole")]
        [ProducesResponseType(typeof(DiscordRole), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(DiscordRole), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(DiscordRole), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMemberHighestRole(string aUserId, CancellationToken aCancellationToken)
            => await _mediator.Send(new GetMemberHighestRoleQuery(Convert.ToUInt64(aUserId)), aCancellationToken)
                .ToActionResult();


        #endregion

        #region Post

        [HttpPost("CreateRole")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostRole(string aRoleName, CancellationToken aCancellationToken)
            => await _mediator.Send(new CreateRoleCommand(aRoleName), aCancellationToken)
                .ToActionResult();


        [HttpPost("CreateCategoryFromTemplate")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostCreateCategoryFromTemplate([FromBody] CategoryChannelTemplate aTemplate, CancellationToken aCancellationToken)
            => await _mediator.Send(new CreateCategoryFromTemplateCommand(aTemplate), aCancellationToken)
                .ToActionResult();


        #endregion

        #region Put

        [HttpPut("AssignRoleToMember")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, CancellationToken aCancellationToken)
            => await _mediator.Send(new AssignRoleToMemberCommand(aRoleId, aFullDiscordHandle), aCancellationToken)
                .ToActionResult();


        [HttpPut("RevokeRoleToMember")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostRevokeRoleToMember(ulong aRoleId, string aFullDiscordHandle, CancellationToken aCancellationToken)
            => await _mediator.Send(new RevokeRoleToMemberCommand(aRoleId, aFullDiscordHandle), aCancellationToken)
                .ToActionResult();

        [HttpPut("AssignRoleToMemberList")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandleList, CancellationToken aCancellationToken)
            => await _mediator.Send(new AssignRoleToMemberListCommand(aRoleId, aFullDiscordHandleList), aCancellationToken)
                .ToActionResult();

        [HttpPut("RevokeRoleToMemberList")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostRevokeRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandle, CancellationToken aCancellationToken)
            => await _mediator.Send(new RevokeRoleToMemberListCommand(aRoleId, aFullDiscordHandle), aCancellationToken)
                .ToActionResult();


        [HttpPut("AddMemberListToCategory")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutAddMemberListToCategory(ulong aCategoryId, string[] aUserFullHandleList, CancellationToken aCancellationToken)
            => await _mediator.Send(new AddMemberListToCategoryCommand(aCategoryId, aUserFullHandleList), aCancellationToken)
                .ToActionResult();


        [HttpPut("UpdateCategoryFromTemplate")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateCategoryFromTemplate(ulong aCategoryId, CategoryChannelTemplate aTemplate, CancellationToken aCancellationToken)
            => await _mediator.Send(new UpdateCategoryFromTemplateCommand(aCategoryId, aTemplate), aCancellationToken)
                .ToActionResult();


        #endregion

        #region Delete

        [HttpDelete("DeleteCategory")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostDeleteCategory(ulong aCategoryId, CancellationToken aCancellationToken)
            => await _mediator.Send(new DeleteCategoryCommand(aCategoryId), aCancellationToken)
                .ToActionResult();


        [HttpDelete("DeleteRole")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken)
            => await _mediator.Send(new DeleteRoleCommand(aRoleId), aCancellationToken)
                .ToActionResult();


        #endregion

    }
}