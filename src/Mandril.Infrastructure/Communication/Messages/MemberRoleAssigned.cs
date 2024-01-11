﻿using Mandril.Application.DTOs;
using TGF.CA.Infrastructure.Communication.Messages;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRoleAssigned(string DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
