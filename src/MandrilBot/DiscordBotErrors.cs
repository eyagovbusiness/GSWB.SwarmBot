using System.Net;
using TGF.Common.ROP.Errors;

namespace MandrilBot
{
    public static class DiscordBotErrors
    {

        public static HttpError BadRequest => new HttpError(
            new Error("BadRequest",
                "The request failed with the current inputs."),
            HttpStatusCode.BadRequest);

        public static class User
        {
            public static HttpError NotFoundId => new HttpError(
                new Error("User.NotFound",
                    "The user with the specified UserId was not found."),
                HttpStatusCode.NotFound);
        }

        public static class Member
        {
            public static HttpError InvalidHandle => new HttpError(
                new Error("User.InvalidHandle",
                    "An invalid member handle provided."),
                HttpStatusCode.BadRequest);

            public static HttpError NotFoundHandle => new HttpError(
                new Error("Member.NotFound",
                    "The member with the specified handle was not found."),
                HttpStatusCode.NotFound);

            public static HttpError OneNotFoundHandle => new HttpError(
                new Error("Member.NotAllUsersFound",
                    "Could not find all members from the specified handles."),
                HttpStatusCode.NotFound);

        }

        public static class Role
        {
            public static HttpError NotFoundId => new HttpError(
             new Error("Role.NotFound",
                "The Role with the specified RoleId was not found.")
            , HttpStatusCode.NotFound);

            public static HttpError RoleNotAssigned => new HttpError(
                new Error("Role.NotAssigned",
                    "Could not assign the specifid Role to the specified User.")
                , HttpStatusCode.InternalServerError);


            public static HttpError RoleNotCreated => new HttpError(
                new Error("Role.NotCreated",
                    "An error happened on creating the specified new Role.")
                , HttpStatusCode.InternalServerError);


            public static HttpError OneUserRoleNotAssigned => new HttpError(
                new Error("Role.NotAllUserAssigned",
                    "Could not assign the specifid Role to all the specified Users."),
                HttpStatusCode.InternalServerError);

        }

        public static class Channel
        {
            public static HttpError NotFound => new HttpError(
                new Error("Channel.NotFound",
                    "Was not found any channel with the specified characteristics."),
                HttpStatusCode.NotFound);
            public static HttpError NotFoundId => new HttpError(
                new Error("Channel.NotFound",
                    "The channel with the specified ChannelId was not found."),
                HttpStatusCode.NotFound);
            public static HttpError NotFoundName => new HttpError(
                new Error("Channel.NotFound",
                    "The channel with the specified name was not found."),
                HttpStatusCode.NotFound);

        }
        public static class Guild
        {
            public static HttpError NotFoundId => new HttpError(
                new Error("Guild.NotFound",
                    "The guild with the specified Id was not found.")
                , HttpStatusCode.NotFound);

        }
        public static class List
        {
            public static HttpError Empty => new HttpError(
                new Error("List.Empty",
                    "The list is empty.")
                , HttpStatusCode.BadRequest);

        }

        public static class Id
        {
            public static HttpError NotValid => new HttpError(
                new Error("Id.NotValid",
                    "The Id is not valid."),
                HttpStatusCode.BadRequest);

        }

    }
}
