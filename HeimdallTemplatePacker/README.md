# Heimdall Templates

The Heimdall template package provides one NuGet installation with a focused project template for each application shape:

- `heimdall-native` — native Heimdall pages and content actions
- `heimdall-mvc` — ASP.NET Core MVC with Heimdall actions
- `heimdall-razor-pages` — ASP.NET Core Razor Pages with Heimdall actions
- `heimdall-blazor` — Blazor components alongside Heimdall actions
- `heimdall-ssg` — static site generation

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
- Output mode: static artifact, ASP.NET Core web root, or hybrid

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

The generated `appsettings.json` contains the `Authentication:Oidc` provider settings. Replace the authority and client values when you enable OIDC, and keep client secrets in environment variables or a secret manager rather than committing them. The scaffold uses cookie sessions, authorization middleware, `/account/sign-in`, and POST `/account/sign-out`. When `--auth oidc` is selected, the hosted application uses a fallback authorization policy so its pages, component endpoints, controllers, and Heimdall actions require an authenticated session by default. Add `[AllowAnonymous]` only for deliberate public endpoints.

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
dotnet new heimdall-ssg -n MyHybridSite --output-mode hybrid
```

The output modes are:

- `artifact`: writes the generated site to `dist/` for deployment to a static host.
- `web-root`: writes the generated site to `wwwroot/` and lets ASP.NET Core serve it.
- `hybrid`: writes the generated pages to `wwwroot/` while keeping live Heimdall pages and actions enabled for the runtime host.

Set `Site:BaseUrl` before generation when you want the template to emit a sitemap. It is intentionally empty by default so a new site does not publish placeholder URLs.

SSG is explicit-route generation. Authentication and per-request user state belong in a hosted or hybrid path, not in pages that must be identical static artifacts.

## Integration Ownership

- `heimdall-native`: Heimdall owns routing and page rendering through explicit server-side render functions.
- `heimdall-mvc`: MVC owns routes and initial views; Heimdall actions can return Razor partial HTML.
- `heimdall-razor-pages`: Razor Pages own routes and page composition; the page model keeps its Heimdall action next to the page and returns a Razor partial.
- `heimdall-blazor`: Blazor owns the component tree and its interactive lifecycle; the page component keeps its Heimdall action inline and returns another Blazor component through the static renderer.
- `heimdall-ssg`: Heimdall registers explicit routes for static generation.

## What Every Hosted Path Includes

- ASP.NET Core host configured for the selected integration
- Heimdall server middleware and browser runtime
- Antiforgery and static asset setup
- Bootstrap styling through Heimdall's typed helpers or integration assets
- A small working example showing how the selected ownership model and Heimdall work together

## Documentation

Full documentation and integration guides:

https://heimdall-framework.org

## License

MIT
