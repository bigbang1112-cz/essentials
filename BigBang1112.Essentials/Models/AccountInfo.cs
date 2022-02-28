namespace BigBang1112.Models;

public record AccountInfo(Guid Guid,
    AccountRole Role = AccountRole.User,
    ManiaPlanetInfo? ManiaPlanet = null);
