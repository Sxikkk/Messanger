using System.Text.Json.Serialization;

namespace Domain.Entities;

public sealed record UserSettings
{
    [JsonIgnore]
    public Guid Id { get; init; }
    [JsonIgnore]
    public Guid UserId { get; init; }
    [JsonIgnore]
    public User? User { get; init; }
    public bool ShowBio { get; set; }
    public bool ShowAvatar { get; set; }

    public UserSettings(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ShowBio = false;
        ShowAvatar = true;
    }
};