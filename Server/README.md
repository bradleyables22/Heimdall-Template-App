## Why Heimdall Exists

Modern web development has largely converged on a default assumption:

Build a SPA.

That approach works well, but it comes with trade-offs:

- Client complexity grows quickly
- UI state is duplicated between client and server
- Simple interactions often require significant frontend infrastructure
- Returning HTML — the browser’s native UI format — becomes a secondary concern

At the same time, many applications do not need a full client application runtime.
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

## When to Use Heimdall (and how to do it)

Heimdall is a strong fit when you want:

-   **HTML over the wire**: actions return HTML fragments, and Heimdall
    swaps them into the DOM
-   **Server-driven interactivity** without building a SPA
-   A **payload-based** model (forms, closest element state, or explicit
    JSON payload)
-   A workflow that can be **stateless-by-default** (send only what you
    need, rehydrate on the server), with an option for **client-carried
    state** when it's convenient
-   **Strongly-typed HTML composition** (FluentHtml builders) while
    keeping the runtime dependency-free
-   **Real-time server push** using **Bifrost (SSE)** for live UI
    updates

### Here's how you do this

#### 1) Wire Heimdall into ASP.NET Core

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();
builder.Services.AddHeimdall();

var app = builder.Build();

app.UseAntiforgery();
app.MapStaticAssets();
app.UseStaticFiles();
app.UseHeimdall();

app.Run();
```

#### 2) Trigger a server action from HTML

``` html
<div id="counter-host" data-heimdall-state='{"count":3}'>
  <button
    type="button"
    heimdall-content-click="CounterPage.Inc"
    heimdall-payload-from="closest-state"
    heimdall-content-target="#counter-host"
    heimdall-content-swap="outer">
    +
  </button>
</div>
```

#### 3) Make a method invokable (server action)

``` csharp
public static class CounterPage
{
    [ContentInvocation("CounterPage.Inc")]
    public static IHtmlContent Inc(CounterState payload)
    {
        var next = payload with { Count = payload.Count + 1 };
        return CounterView.Render(next);
    }
}

public record CounterState(int Count);
```

#### 4) Post a form

``` html
<form heimdall-content-submit="Notes.Create"
      heimdall-content-target="#notes-list"
      heimdall-content-swap="beforeend">
  <input name="title" />
  <button type="submit">Add</button>
</form>

<div id="notes-list"></div>
```

#### 5) Push updates from the server with Bifrost (SSE)

Subscribe from HTML:

``` html
<div heimdall-sse="topic:alerts"
     heimdall-sse-target="#alerts"
     heimdall-sse-swap="afterbegin"></div>

<div id="alerts"></div>
```

Publish from the server:

``` csharp
await bifrost.PublishAsync(
    topic: "topic:alerts",
    content: Html.Tag("div", "New alert!"),
    ttl: TimeSpan.FromSeconds(10),
    ct: ct
);
```
