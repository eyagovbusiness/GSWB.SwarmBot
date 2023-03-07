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
        public async Task GetNumberOfUsersOnline()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //GetNumberOfUsersOnline
            var lGetNumberOfUsersOnlineRply = await TestCommon._httpClient.GetAsync("/Mandril/GetNumberOfOnlineUsers");
            await TestCommon.AssertResponseWithContentAsync(lGetNumberOfUsersOnlineRply, lExpectedStatusCode, (ResultStruct<int> lRes) => lRes.isSuccess && lRes.value >= 0);

            //GetNumberOfUsersOnline
            var lResponse = await TestCommon._httpClient.GetAsync($"/Mandril/GetUserCreationDate?aUserId={1074678110991163402}");
            await TestCommon.AssertResponseWithContentAsync(lResponse, lExpectedStatusCode, (ResultStruct<DateTimeOffset> lRes) => lRes.isSuccess && lRes.value != null);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToAllMembers_ThenRevokeRoleToAllMembers_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CreateNewRole
            var lCreateRoleRply = await TestCommon._httpClient.PostAsync("/Mandril/CreateRole?aRoleName=IntegrationTestRole", default);
            var lCreateRoleResult = await TestCommon.AssertResponseWithContentAsync(lCreateRoleRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //AssignRoleToAllMembers
            var lAssignRoleToMemberListRply = await TestCommon._httpClient.PutAsync($"/Mandril/AssignRoleToMemberList?aRoleId={lCreateRoleResult.value}", TestCommon.GetJsonStringContent(new string[] { "All" }));
            await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //ThenRevokeRoleToAllMembers
            var lRevokeRoleRply = await TestCommon._httpClient.PutAsync($"/Mandril/RevokeRoleToMemberList?aRoleId={lCreateRoleResult.value}", TestCommon.GetJsonStringContent(new string[] { "All" }));
            await TestCommon.AssertResponseWithContentAsync(lRevokeRoleRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //DeleteRole
            var lDeleteRoleRply = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteRole?aRoleId={lCreateRoleResult.value}", default);
            await TestCommon.AssertResponseWithContentAsync(lDeleteRoleRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

        }

        [Test]
        public async Task CheckIfEqualTemplateCategoryExist_ThenCreateCategoryFromTemplate_ThenJoinAllMembersToCategory_ThenCheckIfCreatedCategoryExist_ThenDeleteCategory()
        {
            //init
            CategoryChannelTemplate lTemplateSample = TestHelpers.GetCategoryChannelTemplateSample();
            var lExpectedStatusCode = HttpStatusCode.OK;

            //CheckIfEqualTemplateCategoryExist
            var lGetExistingCategoryIdResponse = await TestCommon._httpClient.GetAsync(string.Format("/Mandril/GetExistingCategoryId?aCategoryName={0}", lTemplateSample.Name));
            await TestCommon.AssertResponseWithContentAsync(lGetExistingCategoryIdResponse, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value == null);

            //CreateCategoryFromTemplate
            var lCreateCategoryFromTemplateResponse = await TestCommon._httpClient.PostAsync("/Mandril/CreateCategoryFromTemplate", TestCommon.GetJsonStringContent(lTemplateSample));
            var lCreateCategoryFromTemplateResult = await TestCommon.AssertResponseWithContentAsync(lCreateCategoryFromTemplateResponse, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //JoinAllMembersToCategory
            var lAddMemberListToCategoryResponse = await TestCommon._httpClient.PutAsync(
                                                                                string.Format("/Mandril/AddMemberListToCategory?aCategoryId={0}", lCreateCategoryFromTemplateResult.value),
                                                                                TestCommon.GetJsonStringContent(new string[] { "All" }));
            var lAddMemberListToCategoryResult = await TestCommon.AssertResponseWithContentAsync(lAddMemberListToCategoryResponse, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

            //CheckIfCreatedCategoryExist
            var lCheckIfCreatedCategoryExistRply = await TestCommon._httpClient.GetAsync(string.Format("/Mandril/GetExistingCategoryId?aCategoryName={0}", lTemplateSample.Name));
            await TestCommon.AssertResponseWithContentAsync(lCheckIfCreatedCategoryExistRply, lExpectedStatusCode, (ResultStruct<string> lRes) => lRes.isSuccess && lRes.value != null);

            //DeleteCategory
            var lDeleteCategoryRply = await TestCommon._httpClient.DeleteAsync(string.Format("/Mandril/DeleteCategory?aCategoryId={0}", lCreateCategoryFromTemplateResult.value));
            await TestCommon.AssertResponseWithContentAsync(lDeleteCategoryRply, lExpectedStatusCode, (ResultStruct lRes) => lRes.isSuccess);

        }
    }
}