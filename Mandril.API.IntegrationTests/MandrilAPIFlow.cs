using System.Net;
using TGF.CA.Domain.Primitives;

namespace Mandril.API.IntegrationTests
{
    public class MandrilAPIFlow
    {
        [Test]
        public async Task GetNumberOfUsersOnline()
        {

            var lExpectedStatusCode = HttpStatusCode.OK;
            var lExpectedContent = new ResultStruct<int>()
            {
                value = 0,
                isSuccess = true,
                error = new Error(default, default)
            };

            var lResponse = await TestCommon._httpClient.GetAsync("/Mandril/GetNumberOfOnlineUsers");
            await TestCommon.AssertResponseWithContentAsync(lResponse, lExpectedStatusCode, lExpectedContent);

        }

        [Test]
        public async Task CreateNewRole_ThenAssignRoleToAllMembers_ThenRevokeRoleToAllMembers_ThenDeleteRole()
        {
            var lExpectedStatusCode = HttpStatusCode.OK;
            var lCreateRoleResponse = await TestCommon._httpClient.PostAsync("/Mandril/CreateRole?aRoleName=IntegrationTestRole", default);
            TestCommon.AssertCommonResponseParts(lCreateRoleResponse, lExpectedStatusCode);

            var lCreateRoleResult = await System.Text.Json.JsonSerializer.DeserializeAsync<ResultStruct<string>>(
                await lCreateRoleResponse.Content.ReadAsStreamAsync());

            var lExpectedContent = new ResultStruct()
            {
                isSuccess = true,
                error = new Error(default, default)
            };

            var lAssignRoleToMemberListResponse = await TestCommon._httpClient.PutAsync($"/Mandril/AssignRoleToMemberList?aRoleId={lCreateRoleResult.value}", TestCommon.GetJsonStringContent(new string[] { "All" }));
            await TestCommon.AssertResponseWithContentAsync(lAssignRoleToMemberListResponse, lExpectedStatusCode, lExpectedContent);

            var lRevokeRoleResponse = await TestCommon._httpClient.PutAsync($"/Mandril/RevokeRoleToMemberList?aRoleId={lCreateRoleResult.value}", TestCommon.GetJsonStringContent(new string[] { "All" }));
            await TestCommon.AssertResponseWithContentAsync(lRevokeRoleResponse, lExpectedStatusCode, lExpectedContent);

            var lDeleteRoleResponse = await TestCommon._httpClient.DeleteAsync($"/Mandril/DeleteRole?aRoleId={lCreateRoleResult.value}", default);
            await TestCommon.AssertResponseWithContentAsync(lDeleteRoleResponse, lExpectedStatusCode, lExpectedContent);

        }

        [Test]
        public async Task CheckIfEqualTemplateCategoryExist_ThenCreateCategoryFromTemplate_ThenJoinAllMembersToCategory_ThenDeleteCategory()
        {
            throw new NotImplementedException();
        }
    }
}