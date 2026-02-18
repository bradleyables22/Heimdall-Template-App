using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Heimdall
{
    /// <summary>
    /// Heimdall-specific HTML helpers for wiring server-rendered markup to Heimdall.js behaviors.
    ///
    /// What this class is for:
    /// - Centralize attribute names used by Heimdall.js (so strings don't drift across the codebase).
    /// - Provide strongly-typed helpers for triggers, payload directives, swap modes, and SSE options.
    /// - Provide safe helpers for embedding JSON payload/state into attributes via System.Text.Json.
    /// - Provide a small "Binding" object that makes common combos ergonomic without losing type safety.
    ///
    /// What this class is NOT:
    /// - A renderer. Rendering/encoding/attribute merging remains in <see cref="Html"/>.
    /// - A DOM framework. It just emits attributes and a small OOB "invocation" instruction element.
    ///
    /// Notes / sanity checks:
    /// - Uses <see cref="Html.Attr(string, string?)"/> so values are HTML-encoded on render.
    /// - Uses presence-only boolean attributes via <see cref="Html.Bool(string, bool)"/> for true/false flags.
    /// - Omits payload/state attrs when null to allow other payload sources to participate.
    /// </summary>
    public static class HeimdallHtml
    {
        // -----------------------------
        // Constants (attribute names)
        // -----------------------------

        /// <summary>
        /// All attribute names Heimdall.js understands.
        /// Keeping them here prevents typos and enables global find/replace if names evolve.
        /// </summary>
        public static class Attrs
        {
            // Content triggers
            public const string Load = "heimdall-content-load";
            public const string Click = "heimdall-content-click";
            public const string Change = "heimdall-content-change";
            public const string Input = "heimdall-content-input";
            public const string Submit = "heimdall-content-submit";
            public const string KeyDown = "heimdall-content-keydown";
            public const string Blur = "heimdall-content-blur";
            public const string Hover = "heimdall-content-hover";
            public const string Visible = "heimdall-content-visible";
            public const string Scroll = "heimdall-content-scroll";

            // Common options
            public const string Target = "heimdall-content-target";
            public const string Swap = "heimdall-content-swap";
            public const string Disable = "heimdall-content-disable";
            public const string PreventDefault = "heimdall-prevent-default";

            // Payload options
            public const string Payload = "heimdall-payload";
            public const string PayloadFrom = "heimdall-payload-from";
            public const string PayloadRef = "heimdall-payload-ref";

            // Trigger modifiers
            public const string Debounce = "heimdall-debounce";
            public const string Key = "heimdall-key";
            public const string HoverDelay = "heimdall-hover-delay";
            public const string VisibleOnce = "heimdall-visible-once";
            public const string ScrollThreshold = "heimdall-scroll-threshold";
            public const string Poll = "heimdall-poll";

            // SSE (Bifrost)
            public const string SseTopic = "heimdall-sse";
            public const string SseTopicAlias = "heimdall-sse-topic";
            public const string SseTarget = "heimdall-sse-target";
            public const string SseSwap = "heimdall-sse-swap";
            public const string SseEvent = "heimdall-sse-event";
            public const string SseDisable = "heimdall-sse-disable";

            // Closest-state payload source (data attributes read by Heimdall.js)
            public const string DataState = "data-heimdall-state";            // un-keyed
            public const string DataStatePrefix = "data-heimdall-state-";     // keyed: data-heimdall-state-foo
        }

        // -----------------------------
        // Strong types
        // -----------------------------

        /// <summary>
        /// Strongly-typed action id, e.g. "Main.Actions.LoadWelcome".
        /// Stored as a string but wrapped to reduce accidental mixing of unrelated strings.
        /// </summary>
        public readonly record struct ActionId(string Value)
        {
            public override string ToString() => Value ?? string.Empty;
            public static implicit operator string(ActionId id) => id.Value;
            public static implicit operator ActionId(string value) => new(value);
        }

        /// <summary>
        /// Heimdall.js trigger categories (maps to specific heimdall-content-* attributes).
        /// </summary>
        public enum Trigger
        {
            Load,
            Click,
            Change,
            Input,
            Submit,
            KeyDown,
            Blur,
            Hover,
            Visible,
            Scroll
        }

        /// <summary>
        /// Swap modes understood by Heimdall.js.
        /// These map to strings: inner|outer|beforeend|afterbegin|none
        /// </summary>
        public enum Swap
        {
            Inner,
            Outer,
            BeforeEnd,
            AfterBegin,
            None
        }

        /// <summary>
        /// Payload-from sources supported by Heimdall.js.
        /// These are string tokens interpreted by the JS runtime.
        /// </summary>
        public static class PayloadFrom
        {
            /// <summary>Serialize form fields from the closest enclosing form.</summary>
            public const string ClosestForm = "closest-form";

            /// <summary>Use the element itself as the source (JS-defined behavior).</summary>
            public const string Self = "self";

            /// <summary>
            /// Read JSON state from nearest ancestor with data-heimdall-state or data-heimdall-state-KEY.
            /// - "closest-state" reads "data-heimdall-state"
            /// - "closest-state:filters" reads "data-heimdall-state-filters"
            /// </summary>
            public const string ClosestState = "closest-state";

            /// <summary>
            /// Builds "closest-state:KEY" (or "closest-state" when key is blank).
            /// </summary>
            public static string ClosestStateKey(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                    return ClosestState;

                return $"{ClosestState}:{key.Trim()}";
            }

            /// <summary>
            /// Reference a global object by path: "ref:Path.To.Object".
            /// (Interpretation depends on Heimdall.js.)
            /// </summary>
            public static string Ref(string globalPath) => $"ref:{globalPath}";
        }

        // -----------------------------
        // Triggers (attribute builders)
        // -----------------------------

        /// <summary>
        /// Generic trigger emitter for any trigger kind.
        /// </summary>
        public static Html.HtmlAttr On(Trigger trigger, ActionId action)
            => Html.Attr(TriggerToAttr(trigger), action.Value);

        /// <summary>Emit heimdall-content-load="ActionId".</summary>
        public static Html.HtmlAttr OnLoad(ActionId action) => Html.Attr(Attrs.Load, action.Value);

        /// <summary>Emit heimdall-content-click="ActionId".</summary>
        public static Html.HtmlAttr OnClick(ActionId action) => Html.Attr(Attrs.Click, action.Value);

        /// <summary>Emit heimdall-content-change="ActionId".</summary>
        public static Html.HtmlAttr OnChange(ActionId action) => Html.Attr(Attrs.Change, action.Value);

        /// <summary>Emit heimdall-content-input="ActionId".</summary>
        public static Html.HtmlAttr OnInput(ActionId action) => Html.Attr(Attrs.Input, action.Value);

        /// <summary>Emit heimdall-content-submit="ActionId".</summary>
        public static Html.HtmlAttr OnSubmit(ActionId action) => Html.Attr(Attrs.Submit, action.Value);

        /// <summary>Emit heimdall-content-keydown="ActionId".</summary>
        public static Html.HtmlAttr OnKeyDown(ActionId action) => Html.Attr(Attrs.KeyDown, action.Value);

        /// <summary>Emit heimdall-content-blur="ActionId".</summary>
        public static Html.HtmlAttr OnBlur(ActionId action) => Html.Attr(Attrs.Blur, action.Value);

        /// <summary>Emit heimdall-content-hover="ActionId".</summary>
        public static Html.HtmlAttr OnHover(ActionId action) => Html.Attr(Attrs.Hover, action.Value);

        /// <summary>Emit heimdall-content-visible="ActionId".</summary>
        public static Html.HtmlAttr OnVisible(ActionId action) => Html.Attr(Attrs.Visible, action.Value);

        /// <summary>Emit heimdall-content-scroll="ActionId".</summary>
        public static Html.HtmlAttr OnScroll(ActionId action) => Html.Attr(Attrs.Scroll, action.Value);

        // -----------------------------
        // Common options
        // -----------------------------

        /// <summary>
        /// Target selector or element reference that Heimdall.js should swap into (commonly "#someId").
        /// </summary>
        public static Html.HtmlAttr Target(string selector) => Html.Attr(Attrs.Target, selector);

        /// <summary>
        /// Sets the swap mode Heimdall.js should use (inner/outer/beforeend/afterbegin/none).
        /// </summary>
        public static Html.HtmlAttr SwapMode(Swap swap) => Html.Attr(Attrs.Swap, SwapToString(swap));

        /// <summary>
        /// Disable element while request is in-flight.
        /// Presence-only attribute; Heimdall.js interprets presence as true.
        /// </summary>
        public static Html.HtmlAttr Disable(bool on = true) => Html.Bool(Attrs.Disable, on);

        /// <summary>
        /// Prevent default browser behavior (anchors/forms/etc).
        /// Presence-only attribute; Heimdall.js interprets presence as true.
        /// </summary>
        public static Html.HtmlAttr PreventDefault(bool on = true) => Html.Bool(Attrs.PreventDefault, on);

        // -----------------------------
        // Payload options
        // -----------------------------

        /// <summary>
        /// Adds a static JSON payload (heimdall-payload="...") serialized with System.Text.Json.
        ///
        /// IMPORTANT:
        /// If payload is null, the attribute is omitted (Empty) so that payload-from / payload-ref can still
        /// participate (based on your Heimdall.js resolution order).
        /// </summary>
        public static Html.HtmlAttr Payload(object? payload, JsonSerializerOptions? options = null)
        {
            if (payload is null)
                return Html.HtmlAttr.Empty;

            var json = JsonSerializer.Serialize(payload, options);
            return Html.Attr(Attrs.Payload, json);
        }

        /// <summary>
        /// Forces an empty JSON object payload: heimdall-payload="{}".
        /// This intentionally overrides payload-from and payload-ref (per your comment).
        /// </summary>
        public static Html.HtmlAttr PayloadEmptyObject()
            => Html.Attr(Attrs.Payload, "{}");

        /// <summary>
        /// Writes payload-from directive (examples: "closest-form", "self", "#form", "closest-state[:key]", "ref:Path.To.Object").
        /// </summary>
        public static Html.HtmlAttr PayloadFromDirective(string from) => Html.Attr(Attrs.PayloadFrom, from);

        /// <summary>
        /// Writes payload-ref directive: the global path string (resolved by Heimdall.js).
        /// </summary>
        public static Html.HtmlAttr PayloadRef(string globalPath) => Html.Attr(Attrs.PayloadRef, globalPath);

        /// <summary>
        /// Convenience helper for payload-from="closest-state" or payload-from="closest-state:key".
        /// </summary>
        public static Html.HtmlAttr PayloadFromClosestState(string? key = null)
            => Html.Attr(Attrs.PayloadFrom, key is null ? PayloadFrom.ClosestState : PayloadFrom.ClosestStateKey(key));

        // -----------------------------
        // Closest-state data attribute writers
        // -----------------------------

        /// <summary>
        /// Writes an un-keyed state JSON blob:
        /// data-heimdall-state='{"page":2}'
        /// </summary>
        public static Html.HtmlAttr State(object? state, JsonSerializerOptions? options = null)
        {
            if (state is null)
                return Html.HtmlAttr.Empty;

            var json = JsonSerializer.Serialize(state, options);
            return Html.Attr(Attrs.DataState, json);
        }

        /// <summary>
        /// Writes a keyed state JSON blob:
        /// data-heimdall-state-filters='{"q":"abc"}'
        /// which can be read with payload-from="closest-state:filters".
        /// </summary>
        public static Html.HtmlAttr State(string key, object? state, JsonSerializerOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("State key is required.", nameof(key));

            if (state is null)
                return Html.HtmlAttr.Empty;

            var json = JsonSerializer.Serialize(state, options);
            return Html.Attr($"{Attrs.DataStatePrefix}{key.Trim()}", json);
        }

        /// <summary>
        /// Advanced: writes raw JSON into data-heimdall-state.
        /// Caller is responsible for passing valid JSON.
        /// </summary>
        public static Html.HtmlAttr StateJson(string json)
            => Html.Attr(Attrs.DataState, json ?? "null");

        /// <summary>
        /// Advanced: writes raw JSON into data-heimdall-state-{key}.
        /// Caller is responsible for passing valid JSON.
        /// </summary>
        public static Html.HtmlAttr StateJson(string key, string json)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("State key is required.", nameof(key));

            return Html.Attr($"{Attrs.DataStatePrefix}{key.Trim()}", json ?? "null");
        }

        // -----------------------------
        // Trigger modifiers
        // -----------------------------

        /// <summary>
        /// Adds heimdall-debounce="{ms}" (clamped to >= 0).
        /// Typically used with input/change style triggers to limit request frequency.
        /// </summary>
        public static Html.HtmlAttr DebounceMs(int ms)
            => Html.Attr(Attrs.Debounce, Math.Max(0, ms).ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Adds heimdall-key="..." (used to match key events, e.g. "Enter").
        /// The exact syntax is defined by Heimdall.js.
        /// </summary>
        public static Html.HtmlAttr Key(string keySpec) => Html.Attr(Attrs.Key, keySpec);

        /// <summary>
        /// Adds heimdall-hover-delay="{ms}" (clamped to >= 0).
        /// Used to delay hover triggers to reduce accidental invocations.
        /// </summary>
        public static Html.HtmlAttr HoverDelayMs(int ms)
            => Html.Attr(Attrs.HoverDelay, Math.Max(0, ms).ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Adds presence-only heimdall-visible-once to run a visible trigger at most once.
        /// </summary>
        public static Html.HtmlAttr VisibleOnce(bool on = true) => Html.Bool(Attrs.VisibleOnce, on);

        /// <summary>
        /// Adds heimdall-scroll-threshold="{px}" (clamped to >= 0).
        /// Used to trigger when near the bottom / threshold (per Heimdall.js behavior).
        /// </summary>
        public static Html.HtmlAttr ScrollThresholdPx(int px)
            => Html.Attr(Attrs.ScrollThreshold, Math.Max(0, px).ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Adds heimdall-poll="{ms}" (clamped to >= 0).
        /// Used to re-trigger certain behaviors (commonly load) on an interval.
        /// </summary>
        public static Html.HtmlAttr PollMs(int ms)
            => Html.Attr(Attrs.Poll, Math.Max(0, ms).ToString(CultureInfo.InvariantCulture));

        // -----------------------------
        // SSE (Bifrost) options
        // -----------------------------

        /// <summary>
        /// Enables SSE binding by setting the topic string (heimdall-sse="topic").
        /// </summary>
        public static Html.HtmlAttr SseTopic(string topic) => Html.Attr(Attrs.SseTopic, topic);

        /// <summary>
        /// Alternate spelling for SSE topic (heimdall-sse-topic="topic").
        /// Keep only if Heimdall.js supports both; otherwise this is just a convenience alias.
        /// </summary>
        public static Html.HtmlAttr SseTopicAlias(string topic) => Html.Attr(Attrs.SseTopicAlias, topic);

        /// <summary>
        /// Sets the target selector for SSE updates (heimdall-sse-target="#id").
        /// </summary>
        public static Html.HtmlAttr SseTarget(string selector) => Html.Attr(Attrs.SseTarget, selector);

        /// <summary>
        /// Sets the swap mode for SSE updates (heimdall-sse-swap="inner|outer|...").
        /// </summary>
        public static Html.HtmlAttr SseSwapMode(Swap swap) => Html.Attr(Attrs.SseSwap, SwapToString(swap));

        /// <summary>
        /// Filters SSE updates to a specific event name (heimdall-sse-event="eventName").
        /// Exact semantics are defined by Heimdall.js.
        /// </summary>
        public static Html.HtmlAttr SseEvent(string eventName) => Html.Attr(Attrs.SseEvent, eventName);

        /// <summary>
        /// Presence-only boolean to disable SSE behavior on an element.
        /// </summary>
        public static Html.HtmlAttr SseDisable(bool on = true) => Html.Bool(Attrs.SseDisable, on);

        // -----------------------------
        // Out-of-band invocation builder
        // -----------------------------

        /// <summary>
        /// Builds an &lt;invocation&gt; element (out-of-band instruction) that Heimdall.js can consume.
        ///
        /// Why template wrapping exists:
        /// Some HTML fragments (notably &lt;tr&gt;) are not preserved when parsing as generic HTML unless
        /// placed inside a &lt;template&gt;. Wrapping allows DOM-safe extraction later.
        ///
        /// Security footnote:
        /// Your JS notes about stripping &lt;script&gt; and allowlisting targets are good; keep those constraints.
        /// </summary>
        /// <param name="targetSelector">Required. e.g. "#toast-manager"</param>
        /// <param name="swap">Optional swap mode. Defaults to Inner.</param>
        /// <param name="payload">Optional payload HTML. If you need table row safety, set wrapInTemplate=true.</param>
        /// <param name="wrapInTemplate">Wrap payload in &lt;template&gt; so fragments like &lt;tr&gt; survive parsing.</param>
        public static IHtmlContent Invocation(
            string targetSelector,
            Swap swap = Swap.Inner,
            IHtmlContent? payload = null,
            bool wrapInTemplate = false)
        {
            if (string.IsNullOrWhiteSpace(targetSelector))
                throw new ArgumentException("Invocation target selector is required.", nameof(targetSelector));

            var swapStr = SwapToString(swap);

            if (payload is null)
            {
                return Html.Tag("invocation",
                    Html.Attr(Attrs.Target, targetSelector),
                    Html.Attr(Attrs.Swap, swapStr)
                );
            }

            if (!wrapInTemplate)
            {
                return Html.Tag("invocation",
                    Html.Attr(Attrs.Target, targetSelector),
                    Html.Attr(Attrs.Swap, swapStr),
                    payload
                );
            }

            return Html.Tag("invocation",
                Html.Attr(Attrs.Target, targetSelector),
                Html.Attr(Attrs.Swap, swapStr),
                Html.Tag("template", payload)
            );
        }

        // -----------------------------
        // Fluent binding object (nice ergonomics)
        // -----------------------------

        /// <summary>
        /// Convenience value object that represents a (Trigger, ActionId) pair and provides fluent helpers
        /// to emit related attributes without repeating "HeimdallHtml." everywhere.
        ///
        /// Example:
        /// <code>
        /// var b = HeimdallHtml.Click("Counter.Inc");
        /// Html.Button(
        ///     b.TriggerAttr(),
        ///     b.Target("#counter-host"),
        ///     b.Swap(Swap.Outer),
        ///     b.PayloadFromClosestState(),
        ///     Html.Text("+")
        /// );
        /// </code>
        /// </summary>
        public readonly record struct Binding(Trigger Trigger, ActionId Action)
        {
            /// <summary>Returns the trigger attribute for this binding (e.g. heimdall-content-click="Action").</summary>
            public Html.HtmlAttr TriggerAttr() => Html.Attr(TriggerToAttr(Trigger), Action.Value);

            /// <summary>Returns heimdall-content-target="...".</summary>
            public Html.HtmlAttr Target(string selector) => HeimdallHtml.Target(selector);

            /// <summary>Returns heimdall-content-swap="...".</summary>
            public Html.HtmlAttr Swap(Swap swap) => HeimdallHtml.SwapMode(swap);

            /// <summary>Returns presence-only heimdall-content-disable.</summary>
            public Html.HtmlAttr Disable(bool on = true) => HeimdallHtml.Disable(on);

            /// <summary>Returns presence-only heimdall-prevent-default.</summary>
            public Html.HtmlAttr PreventDefault(bool on = true) => HeimdallHtml.PreventDefault(on);

            /// <summary>Returns heimdall-debounce="{ms}".</summary>
            public Html.HtmlAttr DebounceMs(int ms) => HeimdallHtml.DebounceMs(ms);

            /// <summary>Returns heimdall-key="...".</summary>
            public Html.HtmlAttr Key(string keySpec) => HeimdallHtml.Key(keySpec);

            /// <summary>Returns heimdall-hover-delay="{ms}".</summary>
            public Html.HtmlAttr HoverDelayMs(int ms) => HeimdallHtml.HoverDelayMs(ms);

            /// <summary>Returns presence-only heimdall-visible-once.</summary>
            public Html.HtmlAttr VisibleOnce(bool on = true) => HeimdallHtml.VisibleOnce(on);

            /// <summary>Returns heimdall-scroll-threshold="{px}".</summary>
            public Html.HtmlAttr ScrollThresholdPx(int px) => HeimdallHtml.ScrollThresholdPx(px);

            /// <summary>Returns heimdall-poll="{ms}".</summary>
            public Html.HtmlAttr PollMs(int ms) => HeimdallHtml.PollMs(ms);

            /// <summary>Returns heimdall-payload="...".</summary>
            public Html.HtmlAttr Payload(object? payload, JsonSerializerOptions? options = null) => HeimdallHtml.Payload(payload, options);

            /// <summary>Returns heimdall-payload="{}".</summary>
            public Html.HtmlAttr PayloadEmptyObject() => HeimdallHtml.PayloadEmptyObject();

            /// <summary>Returns heimdall-payload-from="...".</summary>
            public Html.HtmlAttr PayloadFrom(string from) => HeimdallHtml.PayloadFromDirective(from);

            /// <summary>Returns heimdall-payload-ref="...".</summary>
            public Html.HtmlAttr PayloadRef(string globalPath) => HeimdallHtml.PayloadRef(globalPath);

            /// <summary>Returns payload-from="closest-state" (or closest-state:key).</summary>
            public Html.HtmlAttr PayloadFromClosestState(string? key = null) => HeimdallHtml.PayloadFromClosestState(key);

            /// <summary>Returns a data-heimdall-state="..." attribute from an object.</summary>
            public Html.HtmlAttr State(object? state, JsonSerializerOptions? options = null) => HeimdallHtml.State(state, options);

            /// <summary>Returns a data-heimdall-state-{key}="..." attribute from an object.</summary>
            public Html.HtmlAttr State(string key, object? state, JsonSerializerOptions? options = null) => HeimdallHtml.State(key, state, options);

            /// <summary>
            /// Convenience: returns trigger + target + swap in one call.
            /// Useful for: Html.Button(binding.Apply("#id", Swap.Outer), ...)
            /// </summary>
            public IEnumerable<Html.HtmlAttr> Apply(string targetSelector, Swap swap = Heimdall.HeimdallHtml.Swap.Inner)
            {
                yield return TriggerAttr();
                yield return Target(targetSelector);
                yield return Swap(swap);
            }
        }

        /// <summary>Creates a binding for an arbitrary trigger.</summary>
        public static Binding Bind(Trigger trigger, ActionId action) => new(trigger, action);

        /// <summary>Creates a click binding.</summary>
        public static Binding Click(ActionId action) => new(Trigger.Click, action);

        /// <summary>Creates a load binding.</summary>
        public static Binding Load(ActionId action) => new(Trigger.Load, action);

        /// <summary>Creates a submit binding.</summary>
        public static Binding Submit(ActionId action) => new(Trigger.Submit, action);

        /// <summary>Creates a change binding.</summary>
        public static Binding Change(ActionId action) => new(Trigger.Change, action);

        /// <summary>Creates an input binding.</summary>
        public static Binding Input(ActionId action) => new(Trigger.Input, action);

        /// <summary>Creates a keydown binding.</summary>
        public static Binding KeyDown(ActionId action) => new(Trigger.KeyDown, action);

        /// <summary>Creates a blur binding.</summary>
        public static Binding Blur(ActionId action) => new(Trigger.Blur, action);

        /// <summary>Creates a hover binding.</summary>
        public static Binding Hover(ActionId action) => new(Trigger.Hover, action);

        /// <summary>Creates a visible binding.</summary>
        public static Binding Visible(ActionId action) => new(Trigger.Visible, action);

        /// <summary>Creates a scroll binding.</summary>
        public static Binding Scroll(ActionId action) => new(Trigger.Scroll, action);

        // -----------------------------
        // Helpers
        // -----------------------------

        /// <summary>
        /// Maps <see cref="Trigger"/> to the corresponding Heimdall.js attribute name.
        /// </summary>
        private static string TriggerToAttr(Trigger trigger) => trigger switch
        {
            Trigger.Load => Attrs.Load,
            Trigger.Click => Attrs.Click,
            Trigger.Change => Attrs.Change,
            Trigger.Input => Attrs.Input,
            Trigger.Submit => Attrs.Submit,
            Trigger.KeyDown => Attrs.KeyDown,
            Trigger.Blur => Attrs.Blur,
            Trigger.Hover => Attrs.Hover,
            Trigger.Visible => Attrs.Visible,
            Trigger.Scroll => Attrs.Scroll,
            _ => throw new ArgumentOutOfRangeException(nameof(trigger))
        };

        /// <summary>
        /// Converts <see cref="Swap"/> enum into the exact lowercase token Heimdall.js expects.
        /// Defaults to "inner" for unknown values.
        /// </summary>
        private static string SwapToString(Swap swap) => swap switch
        {
            Swap.Inner => "inner",
            Swap.Outer => "outer",
            Swap.BeforeEnd => "beforeend",
            Swap.AfterBegin => "afterbegin",
            Swap.None => "none",
            _ => "inner"
        };

        // -----------------------------
        // Notes on "missing" / "out of whack"
        // -----------------------------
        //
        // 1) Your using directives:
        //    - Microsoft.AspNetCore.Html is not directly used by this file (it returns IHtmlContent via Html.Tag),
        //      but it's harmless. You could remove it if you want the file leaner.
        //
        // 2) SseTopic vs SseTopicAlias:
        //    - Keeping both is fine *if* Heimdall.js supports both attribute names.
        //      If JS only listens to one, consider making the other an alias helper that returns the supported attr.
        //
        // 3) Trigger coverage:
        //    - Attrs includes Poll and ScrollThreshold; triggers include Scroll; good.
        //    - You have OnX helpers for every Trigger enum value. Good.
        //
        // 4) JSON-in-attributes:
        //    - Html.Attr will encode quotes etc, so attribute emission is safe.
        //    - Attribute size can become a practical limit (huge payload/state). That’s a usage concern, not a code bug.
        //
        // Nothing looked blatantly missing given what Heimdall.js appears to support per your constants.
        //
    }
}
