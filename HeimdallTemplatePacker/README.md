# Heimdall Web App Template

Full ASP.NET Core web application starter for Heimdall's HTML-first rendering style.

Use this template when you want to build pages with strongly typed server-rendered HTML, content invocations, targeted DOM swaps, out-of-band updates, and optional Bifrost SSE without introducing a SPA or client-side component runtime for ordinary UI workflows.

## Install

```powershell
dotnet new install HeimdallFramework.Templates.WebApp
dotnet new heimdall-webapp -n MyHeimdallApp
cd MyHeimdallApp
dotnet run
```

## Documentation

Full documentation:

https://heimdall-framework.org

Related MVC template:

```powershell
dotnet new install HeimdallFramework.Templates.MvcApp
dotnet new heimdall-mvc -n MyHeimdallMvcApp
```

Use the MVC template when controllers, Razor views, and Razor partials should remain the main application structure.

## What You Get

- ASP.NET Core web app configured for Heimdall
- Strongly typed FluentHtml rendering patterns
- Content invocation pipeline wired and working
- Example pages for actions, swaps, forms, state, lazy loading, OOB updates, and SSE toasts
- Bootstrap and Bootstrap Icons assets
- Layout-owned toast manager
- Server-owned UI decisions with a small browser runtime

## Package Versions

The template currently targets:

- `HeimdallFramework.Server` `3.0.0`
- `HeimdallFramework.Web` `3.0.0`
- `HeimdallFramework.Bootstrap` `5.0.0`
- `.NET` `net10.0`

## License

MIT
