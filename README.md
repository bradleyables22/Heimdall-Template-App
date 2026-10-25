# Heimdall Templates

ASP.NET Core starters for Heimdall's strongly typed, HTML-first rendering style. The package is installed once but exposes focused templates for native Heimdall, MVC, Razor Pages, Blazor, and static site generation.

Each generated project is a normal single ASP.NET Core project. The template source and packer remain together in this repository so the integrations can evolve and be smoke-tested as one release unit.

## Install

```powershell
dotnet new install HeimdallFramework.Templates
```

In Visual Studio, search for `Heimdall` and choose the template that matches the integration path you want. Hosted templates offer framework and optional OpenID Connect choices. The static-site template offers framework and output mode choices.

The equivalent command-line templates are:

```powershell
dotnet new heimdall-native -n MyHeimdallApp
dotnet new heimdall-mvc -n MyMvcApp
dotnet new heimdall-razor-pages -n MyRazorPagesApp
dotnet new heimdall-blazor -n MyBlazorApp
dotnet new heimdall-ssg -n MyStaticSite --output-mode artifact
```

Optional hosted OpenID Connect scaffolding is available on the native, MVC, Razor Pages, and Blazor templates:

```powershell
dotnet new heimdall-native -n MyOidcApp --auth oidc
```

The generated `appsettings.json` contains the `Authentication:Oidc` provider settings. Replace the authority and client values when you enable OIDC, and keep client secrets in environment variables or a secret manager rather than committing them. Selecting OIDC also enables a fallback authorization policy, so the hosted application is authenticated by default rather than merely containing an unused provider registration.

## Documentation

https://heimdall-framework.org

## Why Heimdall Exists

Modern web development has largely converged on a default assumption:

Build a SPA.

That approach works well, but it comes with trade-offs:

- Client complexity grows quickly
- UI state is duplicated between client and server
- Simple interactions often require significant frontend infrastructure
- Returning HTML — the browser's native UI format — becomes a secondary concern

Many applications do not need a full client application runtime. They need interaction, not architecture.

Heimdall keeps the server as the source of truth and uses HTML as the transport layer:

**Event → Server Action → HTML → Targeted DOM update**

This allows applications to:

- Keep business logic and UI decisions close together
- Avoid rebuilding state management on the client
- Incrementally update the page without SPA complexity
- Use real-time server push when needed through Bifrost SSE
- Compose UI using strongly typed HTML instead of string templates

Heimdall is not trying to replace every frontend approach. It is for teams and applications that prefer server-driven UI, predictable interaction flows, a minimal client runtime, and a mental model closer to web documents than an app shell.

## Full Documentation Website

https://heimdall-framework.org

## License

MIT
