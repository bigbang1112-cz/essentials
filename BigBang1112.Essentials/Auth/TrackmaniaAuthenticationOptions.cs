using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace BigBang1112.Auth;

public class TrackmaniaAuthenticationOptions : OAuthOptions
{
    public TrackmaniaAuthenticationOptions()
    {
        ClaimsIssuer = TrackmaniaAuthenticationDefaults.Issuer;
        CallbackPath = TrackmaniaAuthenticationDefaults.CallbackPath;
        AuthorizationEndpoint = TrackmaniaAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = TrackmaniaAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = TrackmaniaAuthenticationDefaults.UserInformationEndpoint;
        SaveTokens = true;

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "account_id");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "display_name");
    }
}
