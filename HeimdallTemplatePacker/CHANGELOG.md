# Changelog

## [4.0.0] - Unreleased

### Added

- Separate `heimdall-native`, `heimdall-mvc`, `heimdall-razor-pages`, `heimdall-blazor`, and `heimdall-ssg` templates in one package.
- Artifact and web-root output modes for the `ssg` integration.
- Optional OpenID Connect and cookie-session scaffolding for every hosted integration.
- Visual Studio-specific host metadata with focused Additional information pages for each integration.
- A Razor Pages starter showing ordinary page ownership with an inline page-model content action that renders a Razor partial.
- A Blazor starter showing static SSR components alongside an inline component content action that renders a Blazor component through `IHeimdallBlazorRenderer`.
- Consolidated all integration starters under one package while keeping each generated project focused.
- Shared web-app styling, Bootstrap Icons, favicon assets, navigation shells, and local Bootstrap assets across the hosted starters.

### Updated

- Template projects target the v4 Heimdall server and web packages.
- Generated projects copy only the selected integration source tree.
- OIDC configuration now lives in the standard `appsettings.json`, preserves environment and command-line overrides, avoids unnecessary token storage, and validates local sign-in return paths.
- OIDC variants now apply a fallback authorization policy so generated hosted applications actually require an authenticated session by default.
- Hosted templates enable detailed errors only in development and leave CORS policy configuration explicit for each application.
- SSG output uses packaged local Bootstrap assets instead of a CDN dependency.
- SSG sitemap generation is opt-in through `Site:BaseUrl` instead of publishing a placeholder host by default.
- Package documentation now describes the ownership model for each integration path and the production configuration boundaries.
