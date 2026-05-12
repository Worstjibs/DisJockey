using System;

namespace DisJockey.Shared.DTOs.Shared;

public class BaseMemberDto
{
    public string? DiscordId { get; set; }
    public string? Username { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime DateJoined { get; set; }
}
