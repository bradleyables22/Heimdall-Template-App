# Heimdall Web App Template

⚠️ **Alpha Status**

Heimdall is currently in **ALPHA**.

APIs, naming, and patterns may change as the project evolves.
Real-world usage and feedback are extremely valuable at this stage.

If you try Heimdall and have thoughts — good or bad — please open an issue or discussion:

https://github.com/bradleyables22/Heimdall
---
## How to Install

dotnet new install HeimdallFramework.Templates.WebApp 



## Why Heimdall Exists

Modern web development has largely converged on a default assumption:

Build a SPA.

That approach works well, but it comes with trade-offs:

- Client complexity grows quickly
- UI state is duplicated between client and server
- Simple interactions often require significant frontend infrastructure
- Returning HTML — the browser’s native UI format — becomes a secondary concern

Many applications do not need a full client application runtime.
They need interaction, not architecture.

Heimdall exists to explore a different default.

Instead of moving rendering and orchestration to the client, Heimdall keeps
the server as the source of truth and uses **HTML as the transport layer**.

The model is simple:

**Event → Server Action → HTML → Targeted DOM update**

This allows applications to:

- Keep business logic and UI decisions close together
- Avoid rebuilding state management on the client
- Incrementally update the page without SPA complexity
- Use real-time server push when needed (SSE via Bifrost)
- Compose UI using strongly-typed HTML instead of string templates

Heimdall is not trying to replace every frontend approach.

It exists for teams and applications that prefer:

- Server-driven UI
- Predictable interaction flows
- Minimal client runtime
- Strong typing across the rendering pipeline
- A mental model closer to “web as documents” than “web as app shell”

In short:

Heimdall explores what modern ASP.NET applications look like when
HTML over the wire is treated as a first-class architectural primitive.

---

## When to Use Heimdall (and how to do it)

Heimdall is a strong fit when you want:

- **HTML over the wire**: actions return HTML fragments and Heimdall swaps them into the DOM
- **Server-driven interactivity** without building a SPA
- A **payload-based** model (forms, closest element state, or explicit JSON payload)
- A workflow that can be **stateless-by-default**, with optional client-carried state
- **Strongly-typed HTML composition** (FluentHtml builders) while keeping the runtime dependency-free
- **Real-time server push** using **Bifrost (SSE)** for live UI updates

> **Note on “stateless” and Content Invocations**
>
> Heimdall content invocations can be implemented either as:
>
> - **Static methods** (recommended) — simple, explicit, and aligned with the stateless / functional style Heimdall is aiming for.
> - **DI-resolved methods/services** — totally valid when you need richer orchestration (DB, services, etc.).
>
> Both work; the “default vibe” is to keep actions small, deterministic, and driven by the incoming payload.

### Here’s how you do this

#### 1) Wire Heimdall into ASP.NET Core

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();
builder.Services.AddCors();
builder.Services.AddHeimdall(); // scans entry/calling assemblies by default

var app = builder.Build();

app.UseAntiforgery();
app.MapStaticAssets();
app.UseStaticFiles();

app.UseHeimdall(); // maps Heimdall endpoints

app.MapHeimdallPage("/",(_, ctx) => 
{
    return MainLayout.Render(ctx, MainPage.Render(), "Home", true);
});

app.Run();
```

#### 2) Render a UI trigger with FluentHtml + FluentHeimdall

This renders a button that calls a server action and swaps the returned HTML into `#counter-host`.

```csharp
using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Utilities;

public static class CounterView
{
    private static readonly HeimdallHtml.ActionId Inc = "CounterPage.Inc";

    public static IHtmlContent Render(CounterState state)
        => FluentHtml.Div(host =>
        {
            host.Id("counter-host");

            // Store client-carried state (optional) so actions can be stateless-by-default.
            host.Heimdall().State(state);

            host.Button(btn =>
            {
                btn.Type("button");
                btn.Text("+");

                btn.Heimdall()
                    .OnClick(Inc)
                    .PayloadFromClosestState()
                    .Target("#counter-host")
                    .SwapOuter();
            });
        });
}

public record CounterState(int Count);
```

#### 3) Make a method invokable (server action)

Static action (stateless-by-default style):

```csharp
using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Utilities;

public static class CounterPage
{
    [ContentInvocation] // defaults to ClassName.MethodName if not specified
    public static IHtmlContent Inc(CounterState payload)
    {
        var next = payload with { Count = payload.Count + 1 };

        // Return HTML (fragment or full element). Client swaps it per swap mode.
        return CounterView.Render(next);
    }
}
```

Or DI style (when you need services):

```csharp
using Heimdall.Server;
using Microsoft.AspNetCore.Html;

public sealed class NotesActions
{
    private readonly NotesRepository _repo;

    public NotesActions(NotesRepository repo) => _repo = repo;

    [ContentInvocation("Notes.Create")]
    public async Task<IHtmlContent> Create(NotesCreatePayload payload)
    {
        await _repo.CreateAsync(payload.Title);
        return await _repo.RenderNotesListAsync();
    }
}

public sealed record NotesCreatePayload(string Title);
```

#### 4) Post a form (payload-from closest form)

```csharp
using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Utilities;

public static class NotesPage
{
    private static readonly HeimdallHtml.ActionId Create = "Notes.Create";

    public static IHtmlContent Render()
        => FluentHtml.Fragment(f =>
        {
            f.Form(form =>
            {
                form.Heimdall()
                    .OnSubmit(Create)
                    .PayloadFromClosestForm()
                    .Target("#notes-list")
                    .SwapBeforeEnd();

                form.Input(Html.InputType.text, i =>
                {
                    i.Name("title");
                    i.Class(Bootstrap.Form.Control);
                });

                form.Button(btn =>
                {
                    btn.Type("submit");
                    btn.Class(Bootstrap.Btn.Primary);
                    btn.Text("Add");
                });
            });

            f.Div(d => d.Id("notes-list"));
        });
}
```

#### 5) Push updates from the server with Bifrost (SSE)

Subscribe from HTML you render (no JS framework required):

```csharp
using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Utilities;

public static class AlertsWidget
{
    public static IHtmlContent Render()
        => FluentHtml.Fragment(f =>
        {
            f.Div(sse =>
            {
                sse.Heimdall()
                    .SseTopic("topic:alerts")
                    .SseTarget("#alerts")
                    .SseSwap(HeimdallHtml.Swap.AfterBegin);
            });

            f.Div(d => d.Id("alerts"));
        });
}
```

Publish from the server:

```csharp
using Microsoft.AspNetCore.Html;
using Server.Utilities;
using Heimdall.Server;

public static class AlertPublisher
{
    public static async Task PublishAsync(Bifrost bifrost, CancellationToken ct)
    {
        await bifrost.PublishAsync(
            topic: "topic:alerts",
            content: FluentHtml.Div(d => d.Text("New alert!")),
            ttl: TimeSpan.FromSeconds(10),
            ct: ct
        );
    }
}
```

---

## Example: OOB Invocation vs SSE

If you want an end-to-end example of both patterns (including `<invocation>` and a layout-owned `#toast-manager`), see the `OobPage` sample in the template app.
