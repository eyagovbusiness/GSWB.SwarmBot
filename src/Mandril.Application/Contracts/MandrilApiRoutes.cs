
namespace Mandril.Application
{
    public struct MandrilApiRoutes
    {
        public const string users_exist = "users/exist";
        public const string users_isVerified = "users/isVerified";
        public const string users_creationDate = "users/creationDate";

        public const string members_numberOnline = "members/numberOnline";
        public const string members_profile = "members/profile";
        public const string members_roles = "members/roles";

        public const string roles_create = "roles/create";
        public const string roles_serverRoles = "roles/serverRoles";
        public const string roles_assignToMemberList = "roles/assignToMemberList";
        public const string roles_revokeForMemberList = "roles/revokeForMemberList";
        public const string roles_delete = "roles/delete";

        public const string channels_categories_getId = "channels/categories/getId";
        public const string channels_categories_create = "channels/categories/create";
        public const string channels_categories_addMemberList = "channels/categories/addMemberList";
        public const string channels_categories_update = "channels/categories/update";
        public const string channels_categories_delete = "channels/categories/delete";

    }

}
