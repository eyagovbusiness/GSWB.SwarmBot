using Mandril.Application;
using Mandril.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TGF.Common.Extensions;

namespace Mandril.API.IntegrationTests
{
    /// <summary>
    /// Class with integration tests for MandrilAPI. The program has to be already running before running this tests.
    /// The address has to be configure properly in <see cref="TestCommon"/> with the address where the MandrilAPI application is running.
    /// </summary>
    public class MandrilAPIFlow
    {

        [Test]
        public async Task GetUserExist_ThenGetNumberOfUsersOnline_ThenGetUserCreationDate_ThenGetUserIsVerified()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //GetUserExist
            var lGetUserExistRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.users_exist)}?userId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserExistRply, lExpectedStatusCode, (bool lRes) => lRes);

            //GetUserIsVerified
            var lGetUserIsVerifiedRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.users_isVerified)}?userId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserIsVerifiedRply, lExpectedStatusCode, (bool lRes) => !lRes);

            //GetUserCreationDate
            var lGetUserCreationDateRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.users_creationDate)}?userId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserCreationDateRply, lExpectedStatusCode, (DateTimeOffset lRes) => lRes > DateTimeOffset.MinValue && lRes < DateTimeOffset.MaxValue);

            //GetNumberOfUsersOnline
            var lGetNumberOfUsersOnlineRply = await TestCommon._httpClient.GetAsync(TestCommon.GetFullRoute(MandrilApiRoutes.members_numberOnline));
            _ = await TestCommon.AssertResponseWithContentAsync(lGetNumberOfUsersOnlineRply, lExpectedStatusCode, (int lRes) => lRes >= 0);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToAllMembers_ThenRevokeRoleToAllMembers_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_create)}?roleName=IntegrationTestRole", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //AssignRoleToAllMembers
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_assignToMemberList)}?roleId={lCreateRoleRes}", TestCommon.GetJsonStringContent(new string[3] { "alpacaintrouble#0", "inquisidor211#0", "drewdj#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (object lRes) => true);

            //ThenRevokeRoleToAllMembers
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_revokeForMemberList)}?roleId={lCreateRoleRes}", TestCommon.GetJsonStringContent(new string[3] { "alpacaintrouble#0", "inquisidor211#0", "drewdj#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (object lRes) => true);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_delete)}?roleId={lCreateRoleRes}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (object lRes) => true);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToMember_ThenRevokeRoleToMember_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_create)}?roleName=IntegrationTestRoleSingle", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //AssignRoleToMember
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_assignToMemberList)}?roleId={lCreateRoleRes}", TestCommon.GetJsonStringContent(new string[1] { "inquisidor211#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (object lRes) => true);

            //ThenRevokeRoleToMember
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_revokeForMemberList)}?roleId={lCreateRoleRes}", TestCommon.GetJsonStringContent(new string[1] { "inquisidor211#0" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (object lRes) => true);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.roles_delete)}?roleId={lCreateRoleRes}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (object lRes) => true);

        }

        [Test]
        public async Task CheckIfEqualTemplateCategoryExist_ThenCreateCategoryFromTemplate_ThenJoinAllMembersToCategory_ThenUpdateCategoryFromTemplate_ThenCheckIfCreatedCategoryExist_ThenDeleteCategory()
        {
            //Init
            CategoryChannelTemplateDTO lTemplateSample = TestHelpers.GetCategoryChannelTemplateSample();
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CheckIfEqualTemplateCategoryExist
            var lGetExistingCategoryIdRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.channels_categories_getId)}?categoryName={lTemplateSample.Name}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetExistingCategoryIdRply, HttpStatusCode.NotFound, (ProblemDetails lRes) => lRes?.Status == (int)HttpStatusCode.NotFound, aJsonMediaTypeOverride: "application/problem+json");

            //CreateCategoryFromTemplate
            var lCreateCategoryFromTemplateRply = await TestCommon._httpClient.PostAsync(TestCommon.GetFullRoute(MandrilApiRoutes.channels_categories_create), TestCommon.GetJsonStringContent(lTemplateSample));
            var lCreateCategoryFromTemplateRes = await TestCommon.AssertResponseWithContentAsync(lCreateCategoryFromTemplateRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //JoinAllMembersToCategory
            var lAddMemberListToCategoryRply = await TestCommon._httpClient.PutAsync(
                                                                                $"{TestCommon.GetFullRoute(MandrilApiRoutes.channels_categories_addMemberList)}?categoryId={lCreateCategoryFromTemplateRes}",
                                                                                TestCommon.GetJsonStringContent(new string[] { "All" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAddMemberListToCategoryRply, lExpectedStatusCode, (object lRes) => true);

            //UpdateCategoryFromTemplate
            lTemplateSample.RandomModifyTemplate();
            var lUpdateCategoryFromTemplateRply = await TestCommon._httpClient.PutAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.channels_categories_update)}?categoryId={lCreateCategoryFromTemplateRes}", TestCommon.GetJsonStringContent(lTemplateSample));
            _ = await TestCommon.AssertResponseWithContentAsync(lUpdateCategoryFromTemplateRply, lExpectedStatusCode, (object lRes) => true);

            //CheckIfCreatedCategoryExist
            var lCheckIfCreatedCategoryExistRply = await TestCommon._httpClient.GetAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.channels_categories_getId)}?categoryName={lTemplateSample.Name}");
            _ = await TestCommon.AssertResponseWithContentAsync(lCheckIfCreatedCategoryExistRply, lExpectedStatusCode, (string lRes) => !lRes.IsNullOrEmpty());

            //DeleteCategory
            var lDeleteCategoryRply = await TestCommon._httpClient.DeleteAsync($"{TestCommon.GetFullRoute(MandrilApiRoutes.channels_categories_delete)}?categoryId={lCreateCategoryFromTemplateRes}");
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteCategoryRply, lExpectedStatusCode, (object lRes) => true);

        }

    }
}