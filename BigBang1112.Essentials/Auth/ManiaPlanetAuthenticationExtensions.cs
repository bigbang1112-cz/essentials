using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace BigBang1112.Auth;

public static class ManiaPlanetAuthenticationExtensions
{
    public static AuthenticationBuilder AddManiaPlanet(this AuthenticationBuilder builder, Action<ManiaPlanetAuthenticationOptions> options)
    {
        return builder.AddOAuth<ManiaPlanetAuthenticationOptions, GeneralAuthenticationHandler<ManiaPlanetAuthenticationOptions>>(
            authenticationScheme: ManiaPlanetAuthenticationDefaults.AuthenticationScheme,
            displayName: ManiaPlanetAuthenticationDefaults.DisplayName,
            configureOptions: options);
    }
}
