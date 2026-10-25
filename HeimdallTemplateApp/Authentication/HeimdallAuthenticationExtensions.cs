using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeimdallTemplateApp.Authentication;

public static class HeimdallAuthenticationExtensions
{
    private const string OidcSectionName = "Authentication:Oidc";

    public static IServiceCollection AddHeimdallAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var oidc = configuration.GetSection(OidcSectionName);
        var oidcEnabled = oidc.GetValue<bool>("Enabled");

        var authentication = services.AddAuthentication(options =>
        {
            if (oidcEnabled)
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }
        });

        services.AddAuthorization(options =>
        {
            if (oidcEnabled)
            {
                // The OIDC template is an authenticated starter, not just an
                // authentication registration sample. New endpoints require a
                // signed-in user unless they explicitly opt out.
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            }
        });

        if (!oidcEnabled)
        {
            return services;
        }

        authentication
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = oidc["Authority"] ?? string.Empty;
                options.ClientId = oidc["ClientId"] ?? string.Empty;
                options.ClientSecret = oidc["ClientSecret"] ?? string.Empty;
                options.CallbackPath = oidc["CallbackPath"] ?? "/signin-oidc";
                options.ResponseType = "code";
                options.RequireHttpsMetadata = oidc.GetValue("RequireHttpsMetadata", true);
                options.Scope.Add("openid");
                options.Scope.Add("profile");
            });

        return services;
    }

    public static IEndpointRouteBuilder MapHeimdallAuthentication(
        this IEndpointRouteBuilder endpoints)
    {
        var configuration = endpoints.ServiceProvider.GetRequiredService<IConfiguration>();
        if (!configuration.GetSection(OidcSectionName).GetValue<bool>("Enabled"))
        {
            return endpoints;
        }

        endpoints.MapGet("/account/sign-in", (string? returnUrl) =>
            Results.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = NormalizeReturnUrl(returnUrl)
                },
                [OpenIdConnectDefaults.AuthenticationScheme]))
            .AllowAnonymous();

        endpoints.MapPost("/account/sign-out", () =>
            Results.SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                [
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme
                ]));

        return endpoints;
    }

    private static string NormalizeReturnUrl(string? returnUrl)
        => !string.IsNullOrWhiteSpace(returnUrl)
            && returnUrl.StartsWith("/", StringComparison.Ordinal)
            && !returnUrl.StartsWith("//", StringComparison.Ordinal)
            && !returnUrl.StartsWith("/\\", StringComparison.Ordinal)
                ? returnUrl
                : "/";
}
