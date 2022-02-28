using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace BigBang1112.Auth;

public static class TrackmaniaAuthenticationExtensions
{
    public static AuthenticationBuilder AddTrackmania(this AuthenticationBuilder builder, Action<TrackmaniaAuthenticationOptions> options)
    {
        return builder.AddOAuth<TrackmaniaAuthenticationOptions, GeneralAuthenticationHandler<TrackmaniaAuthenticationOptions>>(
            authenticationScheme: TrackmaniaAuthenticationDefaults.AuthenticationScheme,
            displayName: TrackmaniaAuthenticationDefaults.DisplayName,
            configureOptions: options);
    }
}
