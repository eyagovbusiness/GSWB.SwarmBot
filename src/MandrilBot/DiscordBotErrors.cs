using TGF.CA.Domain.Primitives;

namespace MandrilBot
{
    public static class DiscordBotErrors
    {

        public static Error BadRequest => new Error(
            "BadRequest",
            "The request failed with the current inputs.");

        public static class User
        {
            public static Error NotFoundId => new Error(
                "User.NotFound",
                "The user with the specified UserId was not found.");

        }

        public static class Member
        {
            public static Error NotFoundHandle => new Error(
                "Member.NotFound",
                "The member with the specified handle was not found.");

            public static Error OneNotFoundHandle => new Error(
                "Member.NotAllUsersFound",
                "Could not find all members from the specified handles.");

        }

        public static class Role
        {
            public static Error NotFoundId => new Error(
                "Role.NotFound",
                "The Role with the specified RoleId was not found.");

            public static Error RoleNotAssigned => new Error(
                "Role.NotAssigned",
                "Could not assign the specifid Role to the specified User.");

            public static Error RoleNotCreated => new Error(
                "Role.NotCreated",
                "Could not create a new Role from the specifid name.");

            public static Error OneUserRoleNotAssigned => new Error(
                "Role.NotAllUserAssigned",
                "Could not assign the specifid Role to all the specified Users.");

        }

        public static class Channel
        {
            public static Error NotFoundId => new Error(
                "Channel.NotFound",
                "The channel with the specified ChannelId was not found.");

        }
        public static class Guild
        {
            public static Error NotFoundId => new Error(
                "Guild.NotFound",
                "The guild with the specified Id was not found.");

        }
        public static class List
        {
            public static Error Empty => new Error(
                "List.Empty",
                "The list is empty.");

        }

        public static class Id
        {
            public static Error NotValid => new Error(
                "Id.NotValid",
                "The Id is not valid.");

        }

    }
}
