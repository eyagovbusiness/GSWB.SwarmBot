using SwarmBot.Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TGF.Common.Extensions;
using Common.Application.DTOs.Discord;
using Common.Infrastructure.Communication.ApiRoutes;

namespace SwarmBot.API.IntegrationTests
{
    /// <summary>
    /// Class with integration tests for SwarmBotAPI. The program has to be already running before running this tests.
    /// The address has to be configure properly in <see cref="TestCommon"/> with the address where the SwarmBotAPI application is running.
    /// </summary>
    public class SwarmBotAPIFlow
    {

        [Test]
        public async Task GetUserExist_ThenGetNumberOfUsersOnline_ThenGetUserCreationDate_ThenGetUserIsVerified()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //GetUserExist
            var lGetUserExistRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.users_exist.Replace("{id}", "1074678110991163402"))}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserExistRply, lExpectedStatusCode, (bool lRes) => lRes);

            //GetUserIsVerified
            var lGetUserIsVerifiedRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.users_isVerified.Replace("{id}", "1074678110991163402"))}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserIsVerifiedRply, lExpectedStatusCode, (bool lRes) => !lRes);

            //GetUserCreationDate
            var lGetUserCreationDateRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.users_creationDate.Replace("{id}", "1074678110991163402"))}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserCreationDateRply, lExpectedStatusCode, (DateTimeOffset lRes) => lRes > DateTimeOffset.MinValue && lRes < DateTimeOffset.MaxValue);

            //GetNumberOfUsersOnline
            var lGetNumberOfUsersOnlineRply = await TestCommon._httpClient.GetAsync(TestCommon.GetFullRoute(SwarmBotApiRoutes.members_countOnline));
            _ = await TestCommon.AssertResponseWithContentAsync(lGetNumberOfUsersOnlineRply, lExpectedStatusCode, (int lRes) => lRes >= 0);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToAllMembers_ThenRevokeRoleToAllMembers_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles)}?name=IntegrationTestRole", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //AssignRoleToAllMembers
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles_assign.Replace("{id}", lCreateRoleRes))}", TestCommon.GetJsonStringContent(new string[2] { "alpacaintrouble#0", "inquisidor211#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (object lRes) => true);

            //ThenRevokeRoleToAllMembers
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles_revoke.Replace("{id}", lCreateRoleRes))}", TestCommon.GetJsonStringContent(new string[2] { "alpacaintrouble#0", "inquisidor211#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (object lRes) => true);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles.Replace("{id}", lCreateRoleRes))}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (object lRes) => true);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToMember_ThenRevokeRoleToMember_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles)}?name=IntegrationTestRoleSingle", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //AssignRoleToMember
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles_assign.Replace("{id}", lCreateRoleRes))}", TestCommon.GetJsonStringContent(new string[1] { "inquisidor211#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (object lRes) => true);

            //ThenRevokeRoleToMember
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles_revoke.Replace("{id}", lCreateRoleRes))}", TestCommon.GetJsonStringContent(new string[1] { "inquisidor211#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (object lRes) => true);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.roles.Replace("{id}", lCreateRoleRes))}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (object lRes) => true);

        }

        [Test]
        public async Task CheckIfEqualTemplateCategoryExist_ThenCreateCategoryFromTemplate_ThenJoinAllMembersToCategory_ThenUpdateCategoryFromTemplate_ThenCheckIfCreatedCategoryExist_ThenDeleteCategory()
        {
            //Init
            CategoryChannelTemplateDTO lTemplateSample = TestHelpers.GetCategoryChannelTemplateSample();
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CheckIfEqualTemplateCategoryExist
            var lGetExistingidRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.channels_categories_byName.Replace("{name}", lTemplateSample.Name))}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetExistingidRply, HttpStatusCode.NotFound, (ProblemDetails lRes) => lRes?.Status == (int)HttpStatusCode.NotFound, aJsonMediaTypeOverride: "application/problem+json");

            //CreateCategoryFromTemplate
            var lCreateCategoryFromTemplateRply = await TestCommon._httpClient.PostAsync(TestCommon.GetFullRoute(SwarmBotApiRoutes.channels_categories), TestCommon.GetJsonStringContent(lTemplateSample));
            var lCreateCategoryFromTemplateRes = await TestCommon.AssertResponseWithContentAsync(lCreateCategoryFromTemplateRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //JoinAllMembersToCategory
            var lAddMemberListToCategoryRply = await TestCommon._httpClient.PutAsync(
                                                                                $"{TestCommon.GetFullRoute(SwarmBotApiRoutes.channels_categories_members.Replace("{id}", lCreateCategoryFromTemplateRes))}",
                                                                                TestCommon.GetJsonStringContent(new string[] { "All" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAddMemberListToCategoryRply, lExpectedStatusCode, (object lRes) => true);

            //UpdateCategoryFromTemplate
            lTemplateSample.RandomModifyTemplate();
            var lUpdateCategoryFromTemplateRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.channels_categories.Replace("{id}", lCreateCategoryFromTemplateRes))}", TestCommon.GetJsonStringContent(lTemplateSample));
            _ = await TestCommon.AssertResponseWithContentAsync(lUpdateCategoryFromTemplateRply, lExpectedStatusCode, (object lRes) => true);

            //CheckIfCreatedCategoryExist
            var lCheckIfCreatedCategoryExistRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.channels_categories_byName.Replace("{name}", lTemplateSample.Name))}");
            _ = await TestCommon.AssertResponseWithContentAsync(lCheckIfCreatedCategoryExistRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //DeleteCategory
            var lDeleteCategoryRply = await TestCommon._httpClient.DeleteAsync($"{TestCommon.GetFullRoute(SwarmBotApiRoutes.channels_categories.Replace("{id}", lCreateCategoryFromTemplateRes))}");
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteCategoryRply, lExpectedStatusCode, (object lRes) => true);

        }

    }
}