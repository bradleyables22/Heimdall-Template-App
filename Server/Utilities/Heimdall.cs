using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Heimdall
{
    public static class HeimdallHtml
    {
        // -----------------------------
        // Constants (attribute names)
        // -----------------------------
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

        /// <summary>Strongly-typed action id, e.g. "Main.Actions.LoadWelcome".</summary>
        public readonly record struct ActionId(string Value)
        {
            public override string ToString() => Value ?? string.Empty;
            public static implicit operator string(ActionId id) => id.Value;
            public static implicit operator ActionId(string value) => new(value);
        }

        public enum Trigger
        {
            Load, Click, Change, Input, Submit, KeyDown, Blur, Hover, Visible, Scroll
        }

        /// <summary>Matches Heimdall.js swap modes: inner|outer|beforeend|afterbegin|none</summary>
        public enum Swap
        {
            Inner,
            Outer,
            BeforeEnd,
            AfterBegin,
            None
        }

        /// <summary>Payload-from sources supported by Heimdall.js.</summary>
        public static class PayloadFrom
        {
            public const string ClosestForm = "closest-form";
            public const string Self = "self";

            /// <summary>
            /// Read JSON state from nearest ancestor with data-heimdall-state or data-heimdall-state-KEY.
            /// - "closest-state" reads "data-heimdall-state"
            /// - "closest-state:filters" reads "data-heimdall-state-filters"
            /// </summary>
            public const string ClosestState = "closest-state";

            public static string ClosestStateKey(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                    return ClosestState;

                return $"{ClosestState}:{key.Trim()}";
            }

            /// <summary>Reference a global object by path: "ref:Path.To.Object"</summary>
            public static string Ref(string globalPath) => $"ref:{globalPath}";
        }

        // -----------------------------
        // Triggers (attribute builders)
        // -----------------------------

        public static Html.HtmlAttr On(Trigger trigger, ActionId action)
            => Html.Attr(TriggerToAttr(trigger), action.Value);

        public static Html.HtmlAttr OnLoad(ActionId action) => Html.Attr(Attrs.Load, action.Value);
        public static Html.HtmlAttr OnClick(ActionId action) => Html.Attr(Attrs.Click, action.Value);
        public static Html.HtmlAttr OnChange(ActionId action) => Html.Attr(Attrs.Change, action.Value);
        public static Html.HtmlAttr OnInput(ActionId action) => Html.Attr(Attrs.Input, action.Value);
        public static Html.HtmlAttr OnSubmit(ActionId action) => Html.Attr(Attrs.Submit, action.Value);
        public static Html.HtmlAttr OnKeyDown(ActionId action) => Html.Attr(Attrs.KeyDown, action.Value);
        public static Html.HtmlAttr OnBlur(ActionId action) => Html.Attr(Attrs.Blur, action.Value);
        public static Html.HtmlAttr OnHover(ActionId action) => Html.Attr(Attrs.Hover, action.Value);
        public static Html.HtmlAttr OnVisible(ActionId action) => Html.Attr(Attrs.Visible, action.Value);
        public static Html.HtmlAttr OnScroll(ActionId action) => Html.Attr(Attrs.Scroll, action.Value);

        // -----------------------------
        // Common options
        // -----------------------------

        /// <summary>Target selector or element reference (usually selector like "#id").</summary>
        public static Html.HtmlAttr Target(string selector) => Html.Attr(Attrs.Target, selector);

        public static Html.HtmlAttr SwapMode(Swap swap) => Html.Attr(Attrs.Swap, SwapToString(swap));

        /// <summary>
        /// Disable element while request in-flight.
        /// Uses presence-only attribute which Heimdall.js treats as true.
        /// </summary>
        public static Html.HtmlAttr Disable(bool on = true) => Html.Bool(Attrs.Disable, on);

        /// <summary>
        /// Prevent default browser behavior (e.g. for anchors/forms).
        /// Uses presence-only attribute which Heimdall.js treats as true.
        /// </summary>
        public static Html.HtmlAttr PreventDefault(bool on = true) => Html.Bool(Attrs.PreventDefault, on);

        // -----------------------------
        // Payload options
        // -----------------------------

        /// <summary>
        /// Static JSON payload (serialized with System.Text.Json).
        ///
        /// IMPORTANT: If payload is null, we OMIT the attribute so that:
        /// - heimdall-payload-from
        /// - heimdall-payload-ref
        /// can still participate (per payloadFromElement in Heimdall.js).
        /// </summary>
        public static Html.HtmlAttr Payload(object? payload, JsonSerializerOptions? options = null)
        {
            if (payload is null)
                return Html.HtmlAttr.Empty;

            var json = JsonSerializer.Serialize(payload, options);
            return Html.Attr(Attrs.Payload, json);
        }

        /// <summary>
        /// Force an empty JSON object payload (heimdall-payload="{}").
        /// This will override payload-from and payload-ref in Heimdall.js, by design.
        /// </summary>
        public static Html.HtmlAttr PayloadEmptyObject()
            => Html.Attr(Attrs.Payload, "{}");

        /// <summary>Payload-from directive: "closest-form", "self", "#form", "closest-state[:key]" or "ref:Path.To.Object".</summary>
        public static Html.HtmlAttr PayloadFromDirective(string from) => Html.Attr(Attrs.PayloadFrom, from);

        /// <summary>Payload-ref directive: "Path.To.Object" (resolved off window/global).</summary>
        public static Html.HtmlAttr PayloadRef(string globalPath) => Html.Attr(Attrs.PayloadRef, globalPath);

        /// <summary>Convenience: payload-from="closest-state" or payload-from="closest-state:key".</summary>
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
        /// and can be read with payload-from="closest-state:filters".
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
        /// For advanced scenarios where you already have JSON (must be valid JSON).
        /// </summary>
        public static Html.HtmlAttr StateJson(string json)
            => Html.Attr(Attrs.DataState, json ?? "null");

        /// <summary>
        /// For advanced scenarios where you already have JSON (must be valid JSON).
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

        public static Html.HtmlAttr DebounceMs(int ms)
            => Html.Attr(Attrs.Debounce, Math.Max(0, ms).ToString(CultureInfo.InvariantCulture));

        public static Html.HtmlAttr Key(string keySpec) => Html.Attr(Attrs.Key, keySpec);

        public static Html.HtmlAttr HoverDelayMs(int ms)
            => Html.Attr(Attrs.HoverDelay, Math.Max(0, ms).ToString(CultureInfo.InvariantCulture));

        /// <summary>Presence-only boolean.</summary>
        public static Html.HtmlAttr VisibleOnce(bool on = true) => Html.Bool(Attrs.VisibleOnce, on);

        public static Html.HtmlAttr ScrollThresholdPx(int px)
            => Html.Attr(Attrs.ScrollThreshold, Math.Max(0, px).ToString(CultureInfo.InvariantCulture));

        /// <summary>Enable polling for heimdall-content-load triggers, in ms.</summary>
        public static Html.HtmlAttr PollMs(int ms)
            => Html.Attr(Attrs.Poll, Math.Max(0, ms).ToString(CultureInfo.InvariantCulture));

        // -----------------------------
        // SSE (Bifrost) options
        // -----------------------------

        public static Html.HtmlAttr SseTopic(string topic) => Html.Attr(Attrs.SseTopic, topic);

        /// <summary>Alias for topic attribute (some folks prefer explicit naming).</summary>
        public static Html.HtmlAttr SseTopicAlias(string topic) => Html.Attr(Attrs.SseTopicAlias, topic);

        public static Html.HtmlAttr SseTarget(string selector) => Html.Attr(Attrs.SseTarget, selector);

        public static Html.HtmlAttr SseSwapMode(Swap swap) => Html.Attr(Attrs.SseSwap, SwapToString(swap));

        public static Html.HtmlAttr SseEvent(string eventName) => Html.Attr(Attrs.SseEvent, eventName);

        /// <summary>Presence-only boolean.</summary>
        public static Html.HtmlAttr SseDisable(bool on = true) => Html.Bool(Attrs.SseDisable, on);

        // -----------------------------
        // Out-of-band invocation builder
        // -----------------------------

        /// <summary>
        /// Builds an &lt;invocation&gt; element (OOB instruction).
        /// Use with care: the JS strips &lt;script&gt; and can allowlist targets.
        /// </summary>
        /// <param name="targetSelector">Required. e.g. "#toast-manager"</param>
        /// <param name="swap">Optional swap mode. Defaults to Inner.</param>
        /// <param name="payload">Optional payload HTML. If you need table row safety, set wrapInTemplate=true.</param>
        /// <param name="wrapInTemplate">Wrap payload in &lt;template&gt; so fragments like &lt;tr&gt; survive parsing.</param>
        public static IHtmlContent Invocation(string targetSelector, Swap swap = Swap.Inner, IHtmlContent? payload = null, bool wrapInTemplate = false)
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

        public readonly record struct Binding(Trigger Trigger, ActionId Action)
        {
            public Html.HtmlAttr TriggerAttr() => Html.Attr(TriggerToAttr(Trigger), Action.Value);
            public Html.HtmlAttr Target(string selector) => HeimdallHtml.Target(selector);
            public Html.HtmlAttr Swap(Swap swap) => HeimdallHtml.SwapMode(swap);
            public Html.HtmlAttr Disable(bool on = true) => HeimdallHtml.Disable(on);
            public Html.HtmlAttr PreventDefault(bool on = true) => HeimdallHtml.PreventDefault(on);

            public Html.HtmlAttr DebounceMs(int ms) => HeimdallHtml.DebounceMs(ms);
            public Html.HtmlAttr Key(string keySpec) => HeimdallHtml.Key(keySpec);
            public Html.HtmlAttr HoverDelayMs(int ms) => HeimdallHtml.HoverDelayMs(ms);
            public Html.HtmlAttr VisibleOnce(bool on = true) => HeimdallHtml.VisibleOnce(on);
            public Html.HtmlAttr ScrollThresholdPx(int px) => HeimdallHtml.ScrollThresholdPx(px);
            public Html.HtmlAttr PollMs(int ms) => HeimdallHtml.PollMs(ms);

            public Html.HtmlAttr Payload(object? payload, JsonSerializerOptions? options = null) => HeimdallHtml.Payload(payload, options);
            public Html.HtmlAttr PayloadEmptyObject() => HeimdallHtml.PayloadEmptyObject();
            public Html.HtmlAttr PayloadFrom(string from) => HeimdallHtml.PayloadFromDirective(from);
            public Html.HtmlAttr PayloadRef(string globalPath) => HeimdallHtml.PayloadRef(globalPath);
            public Html.HtmlAttr PayloadFromClosestState(string? key = null) => HeimdallHtml.PayloadFromClosestState(key);

            public Html.HtmlAttr State(object? state, JsonSerializerOptions? options = null) => HeimdallHtml.State(state, options);
            public Html.HtmlAttr State(string key, object? state, JsonSerializerOptions? options = null) => HeimdallHtml.State(key, state, options);

            /// <summary>Convenience: returns trigger + target + swap in one call.</summary>
            public IEnumerable<Html.HtmlAttr> Apply(string targetSelector, Swap swap = HeimdallHtml.Swap.Inner)
            {
                yield return TriggerAttr();
                yield return Target(targetSelector);
                yield return Swap(swap);
            }
        }

        public static Binding Bind(Trigger trigger, ActionId action) => new(trigger, action);
        public static Binding Click(ActionId action) => new(Trigger.Click, action);
        public static Binding Load(ActionId action) => new(Trigger.Load, action);
        public static Binding Submit(ActionId action) => new(Trigger.Submit, action);
        public static Binding Change(ActionId action) => new(Trigger.Change, action);
        public static Binding Input(ActionId action) => new(Trigger.Input, action);
        public static Binding KeyDown(ActionId action) => new(Trigger.KeyDown, action);
        public static Binding Blur(ActionId action) => new(Trigger.Blur, action);
        public static Binding Hover(ActionId action) => new(Trigger.Hover, action);
        public static Binding Visible(ActionId action) => new(Trigger.Visible, action);
        public static Binding Scroll(ActionId action) => new(Trigger.Scroll, action);

        // -----------------------------
        // Helpers
        // -----------------------------

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

        private static string SwapToString(Swap swap) => swap switch
        {
            Swap.Inner => "inner",
            Swap.Outer => "outer",
            Swap.BeforeEnd => "beforeend",
            Swap.AfterBegin => "afterbegin",
            Swap.None => "none",
            _ => "inner"
        };
    }
}
