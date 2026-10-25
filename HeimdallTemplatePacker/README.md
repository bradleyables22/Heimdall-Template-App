# Heimdall Templates

The Heimdall template package provides one NuGet installation with a focused project template for each application shape:

- `heimdall-native` — native Heimdall pages and content actions
- `heimdall-mvc` — ASP.NET Core MVC with Heimdall actions
- `heimdall-razor-pages` — ASP.NET Core Razor Pages with Heimdall actions
- `heimdall-blazor` — statically rendered Blazor components alongside Heimdall actions
- `heimdall-ssg` — static site generator

The package contains separate template entries so Visual Studio can present only the options that apply to the selected integration. The generated output remains one ordinary ASP.NET Core project; the templates share one source repository and packer solution.

## Install

```powershell
dotnet new install HeimdallFramework.Templates
```

In Visual Studio, search for `Heimdall` in Create a new project. Choose the integration-specific template that matches the ownership model you want.

The hosted templates expose:

- Target framework
- Authentication: none or OpenID Connect

The static-site template exposes:

- Target framework
- Output mode: static artifact or ASP.NET Core web root

## Command Line

Native Heimdall:

```powershell
dotnet new heimdall-native -n MyHeimdallApp
cd MyHeimdallApp
dotnet run
```

MVC, Razor Pages, and Blazor:

```powershell
dotnet new heimdall-mvc -n MyMvcApp
dotnet new heimdall-razor-pages -n MyRazorPagesApp
dotnet new heimdall-blazor -n MyBlazorApp
```

Add the optional hosted OpenID Connect scaffold to any hosted template:

```powershell
dotnet new heimdall-native -n MyOidcApp --auth oidc
```

Selecting `--auth oidc` adds the complete authentication scaffold: the OIDC package reference, authentication extension, provider settings, cookie sessions, authorization middleware, `/account/sign-in`, and POST `/account/sign-out`. Selecting the default `none` choice omits those files, references, routes, configuration, and UI branches entirely. Replace the authority and client values when you enable OIDC, and keep client secrets in environment variables or a secret manager rather than committing them. The OIDC scaffold uses a fallback authorization policy so its pages, component endpoints, controllers, and Heimdall actions require an authenticated session by default. Add `[AllowAnonymous]` only for deliberate public endpoints.

For example:

```powershell
$env:Authentication__Oidc__Authority = "https://login.example.com"
$env:Authentication__Oidc__ClientId = "your-client-id"
$env:Authentication__Oidc__ClientSecret = "your-client-secret"
```

## Static Site Generation

```powershell
dotnet new heimdall-ssg -n MyStaticSite --output-mode artifact
dotnet new heimdall-ssg -n MyHostedStaticSite --output-mode web-root
```

The output modes are:

- `artifact`: writes the generated site to `dist/` for deployment to a static host.
- `web-root`: writes the generated site to `wwwroot/` and lets ASP.NET Core serve it.

Set `Site:BaseUrl` before generation when you want the template to emit a sitemap. It is intentionally empty by default so a new site does not publish placeholder URLs.

SSG is explicit-route generation. Authentication and per-request user state belong in a hosted application, not in pages that must be identical static artifacts.

## Integration Ownership

- `heimdall-native`: Heimdall owns routing and page rendering through explicit server-side render functions.
- `heimdall-mvc`: MVC owns routes and initial views; Heimdall actions can return Razor partial HTML.
- `heimdall-razor-pages`: Razor Pages own routes and page composition; the page model keeps its Heimdall action next to the page and returns a Razor partial.
- `heimdall-blazor`: Blazor owns the component tree during static SSR; Heimdall owns browser interactions, and the page component keeps its content action inline while returning another Blazor component through the static renderer.
- `heimdall-ssg`: Heimdall registers explicit routes for static generation.

## What Every Hosted Path Includes

- ASP.NET Core host configured for the selected integration
- Heimdall server middleware and browser runtime
- Antiforgery and static asset setup
- Bootstrap styling through Heimdall's typed helpers or integration assets
- A small working example showing how the selected ownership model and Heimdall work together

Each hosted starter also includes `wwwroot/js/heimdall.config.js`, loaded after the Heimdall browser bundle. Use it for application-wide browser defaults such as request timeouts, client-information opt-in, debug logging during development, or an asynchronous request-header provider. Keep page-specific behavior in the rendered Heimdall attributes, and never place secrets in this file.

## Documentation

Full documentation and integration guides:

https://heimdall-framework.org

## License

MIT
