using MandrilBot;
using System.Net;

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
            var lGetUserExistRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetUserExist?aUserId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserExistRply, lExpectedStatusCode, (Result<bool> lRes) => lRes.IsSuccess && lRes.Value);

            //GetUserIsVerified
            var lGetUserIsVerifiedRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetUserIsVerified?aUserId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserIsVerifiedRply, lExpectedStatusCode, (Result<bool> lRes) => lRes.IsSuccess && !lRes.Value);

            //GetUserCreationDate
            var lGetUserCreationDateRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetUserCreationDate?aUserId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserCreationDateRply, lExpectedStatusCode, (Result<DateTimeOffset> lRes) => lRes.IsSuccess && lRes.Value > DateTimeOffset.MinValue && lRes.Value < DateTimeOffset.MaxValue);

            //GetNumberOfUsersOnline
            var lGetNumberOfUsersOnlineRply = await TestCommon._httpClient.GetAsync("/Mandril/GetNumberOfOnlineMembers");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetNumberOfUsersOnlineRply, lExpectedStatusCode, (Result<int> lRes) => lRes.IsSuccess && lRes.Value >= 0);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToAllMembers_ThenRevokeRoleToAllMembers_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateRole?aRoleName=IntegrationTestRole", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (Result<string> lRes) => lRes.IsSuccess && lRes.Value != null);

            //AssignRoleToAllMembers
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"/Mandril/AssignRoleToMemberList?aRoleId={lCreateRoleRes.Value}", TestCommon.GetJsonStringContent(new string[3] { "AlpacaInTrouble#3724", "HawK#4447", "Ragh#5567" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

            //ThenRevokeRoleToAllMembers
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"/Mandril/RevokeRoleToMemberList?aRoleId={lCreateRoleRes.Value}", TestCommon.GetJsonStringContent(new string[3] { "AlpacaInTrouble#3724", "HawK#4447", "Ragh#5567" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteRole?aRoleId={lCreateRoleRes.Value}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToMember_ThenRevokeRoleToMember_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateRole?aRoleName=IntegrationTestRoleSingle", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (Result<string> lRes) => lRes.IsSuccess && lRes.Value != null);

            //AssignRoleToMember
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"/Mandril/AssignRoleToMember?aRoleId={lCreateRoleRes.Value}&aFullDiscordHandle=MandrilBot%230739", TestCommon.GetJsonStringContent(""));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

            //ThenRevokeRoleToMember
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"/Mandril/RevokeRoleToMember?aRoleId={lCreateRoleRes.Value}&aFullDiscordHandle=MandrilBot%230739", TestCommon.GetJsonStringContent(""));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteRole?aRoleId={lCreateRoleRes.Value}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

        }

        [Test]
        public async Task CheckIfEqualTemplateCategoryExist_ThenCreateCategoryFromTemplate_ThenJoinAllMembersToCategory_ThenUpdateCategoryFromTemplate_ThenCheckIfCreatedCategoryExist_ThenDeleteCategory()
        {
            //Init
            CategoryChannelTemplate lTemplateSample = TestHelpers.GetCategoryChannelTemplateSample();
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CheckIfEqualTemplateCategoryExist
            var lGetExistingCategoryIdRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetExistingCategoryId?aCategoryName={lTemplateSample.Name}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetExistingCategoryIdRply, HttpStatusCode.NotFound, (Result<string> lRes) => !lRes.IsSuccess && lRes.Value == null);

            //CreateCategoryFromTemplate
            var lCreateCategoryFromTemplateRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateCategoryFromTemplate", TestCommon.GetJsonStringContent(lTemplateSample));
            var lCreateCategoryFromTemplateRes = await TestCommon.AssertResponseWithContentAsync(lCreateCategoryFromTemplateRply, lExpectedStatusCode, (Result<string> lRes) => lRes.IsSuccess && lRes.Value != null);

            //JoinAllMembersToCategory
            var lAddMemberListToCategoryRply = await TestCommon._httpClient.PutAsync(
                                                                                $"/Mandril/AddMemberListToCategory?aCategoryId={lCreateCategoryFromTemplateRes.Value}",
                                                                                TestCommon.GetJsonStringContent(new string[] { "All" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAddMemberListToCategoryRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

            //UpdateCategoryFromTemplate
            lTemplateSample.RandomModifyTemplate();
            var lUpdateCategoryFromTemplateRply = await TestCommon._httpClient.PutAsync($"/Mandril/UpdateCategoryFromTemplate?aCategoryId={lCreateCategoryFromTemplateRes.Value}", TestCommon.GetJsonStringContent(lTemplateSample));
            _ = await TestCommon.AssertResponseWithContentAsync(lUpdateCategoryFromTemplateRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

            //CheckIfCreatedCategoryExist
            var lCheckIfCreatedCategoryExistRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetExistingCategoryId?aCategoryName={lTemplateSample.Name}");
            _ = await TestCommon.AssertResponseWithContentAsync(lCheckIfCreatedCategoryExistRply, lExpectedStatusCode, (Result<string> lRes) => lRes.IsSuccess && lRes.Value != null);

            //DeleteCategory
            var lDeleteCategoryRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteCategory?aCategoryId={lCreateCategoryFromTemplateRes.Value}");
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteCategoryRply, lExpectedStatusCode, (Result<object> lRes) => lRes.IsSuccess);

        }

    }
}