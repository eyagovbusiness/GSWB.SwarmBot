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
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserExistRply, lExpectedStatusCode, (ResultStruct<bool> lRes) => lRes.isSuccess && lRes.value);

            //GetUserIsVerified
            var lGetUserIsVerifiedRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetUserIsVerified?aUserId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserIsVerifiedRply, lExpectedStatusCode, (ResultStruct<bool> lRes) => lRes.isSuccess && !lRes.value);

            //GetUserCreationDate
            var lGetUserCreationDateRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetUserCreationDate?aUserId={1074678110991163402}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetUserCreationDateRply, lExpectedStatusCode, (ResultStruct<DateTimeOffset> lRes) => lRes.isSuccess && lRes.value > DateTimeOffset.MinValue && lRes.value < DateTimeOffset.MaxValue);

            //GetNumberOfUsersOnline
            var lGetNumberOfUsersOnlineRply = await TestCommon._httpClient.GetAsync("/Mandril/GetNumberOfOnlineUsers");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetNumberOfUsersOnlineRply, lExpectedStatusCode, (ResultStruct<int> lRes) => lRes.isSuccess && lRes.value >= 0);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToAllMembers_ThenRevokeRoleToAllMembers_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateRole?aRoleName=IntegrationTestRole", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //AssignRoleToAllMembers
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"/Mandril/AssignRoleToMemberList?aRoleId={lCreateRoleRes.value}", TestCommon.GetJsonStringContent(new string[] { "All" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //ThenRevokeRoleToAllMembers
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"/Mandril/RevokeRoleToMemberList?aRoleId={lCreateRoleRes.value}", TestCommon.GetJsonStringContent(new string[] { "All" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteRole?aRoleId={lCreateRoleRes.value}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToMember_ThenRevokeRoleToMember_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateRole?aRoleName=IntegrationTestRoleSingle", default);
            var lCreateRoleRes = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //AssignRoleToAllMembers
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"/Mandril/AssignRoleToMember?aRoleId={lCreateRoleRes.value}&aFullDiscordHandle=MandrilBot#0739", TestCommon.GetJsonStringContent(""));
            _ = await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //ThenRevokeRoleToAllMembers
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"/Mandril/RevokeRoleToMember?aRoleId={lCreateRoleRes.value}&aFullDiscordHandle=MandrilBot#0739", TestCommon.GetJsonStringContent(""));
            _ = await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteRole?aRoleId={lCreateRoleRes.value}", default);
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

        }

        [Test]
        public async Task CheckIfEqualTemplateCategoryExist_ThenCreateCategoryFromTemplate_ThenJoinAllMembersToCategory_ThenUpdateCategoryFromTemplate_ThenCheckIfCreatedCategoryExist_ThenDeleteCategory()
        {
            //Init
            CategoryChannelTemplate lTemplateSample = TestHelpers.GetCategoryChannelTemplateSample();
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CheckIfEqualTemplateCategoryExist
            var lGetExistingCategoryIdRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetExistingCategoryId?aCategoryName={lTemplateSample.Name}");
            _ = await TestCommon.AssertResponseWithContentAsync(lGetExistingCategoryIdRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value == null);

            //CreateCategoryFromTemplate
            var lCreateCategoryFromTemplateRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateCategoryFromTemplate", TestCommon.GetJsonStringContent(lTemplateSample));
            var lCreateCategoryFromTemplateRes = await TestCommon.AssertResponseWithContentAsync(lCreateCategoryFromTemplateRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //JoinAllMembersToCategory
            var lAddMemberListToCategoryRply = await TestCommon._httpClient.PutAsync(
                                                                                $"/Mandril/AddMemberListToCategory?aCategoryId={lCreateCategoryFromTemplateRes.value}",
                                                                                TestCommon.GetJsonStringContent(new string[] { "All" }));
            _ = await TestCommon.AssertResponseWithContentAsync(lAddMemberListToCategoryRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //UpdateCategoryFromTemplate
            lTemplateSample.RandomModifyTemplate();
            var lUpdateCategoryFromTemplateRply = await TestCommon._httpClient.PutAsync($"/Mandril/UpdateCategoryFromTemplate?aCategoryId={lCreateCategoryFromTemplateRes.value}", TestCommon.GetJsonStringContent(lTemplateSample));
            _ = await TestCommon.AssertResponseWithContentAsync(lUpdateCategoryFromTemplateRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //CheckIfCreatedCategoryExist
            var lCheckIfCreatedCategoryExistRply = await TestCommon._httpClient.GetAsync($"/Mandril/GetExistingCategoryId?aCategoryName={lTemplateSample.Name}");
            _ = await TestCommon.AssertResponseWithContentAsync(lCheckIfCreatedCategoryExistRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //DeleteCategory
            var lDeleteCategoryRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteCategory?aCategoryId={lCreateCategoryFromTemplateRes.value}");
            _ = await TestCommon.AssertResponseWithContentAsync(lDeleteCategoryRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

        }

    }
}