using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Heimdall
{
	/// <summary>
	/// Fluent extension layer for applying HeimdallHtml attributes to FluentHtml builders.
	///
	/// Goals:
	/// - Enterprise-friendly: predictable, minimal magic, easy to debug
	/// - Zero duplication of rules: delegates to HeimdallHtml for attribute construction
	/// - Ergonomic at call sites: enables "builder.Heimdall().Click(...).Target(...).Swap(...)" patterns
	///
	/// Notes:
	/// - This file intentionally avoids creating a new DSL; it is a thin wrapper over HeimdallHtml.
	/// - All methods return the wrapper for chainability.
	/// </summary>
	public static class FluentHeimdall
	{
		// ---------------------------------------------------------------------
		// Entry points
		// ---------------------------------------------------------------------

		/// <summary>Starts a Heimdall fluent chain for a FluentHtml element builder.</summary>
		public static HeimdallBuilder Heimdall(this FluentHtml.ElementBuilder b) => new(b);

		/// <summary>
		/// Starts a Heimdall fluent chain for a FluentHtml fragment builder.
		/// Useful when building fragments that include invocations, templates, etc.
		/// </summary>
		public static HeimdallFragmentBuilder Heimdall(this FluentHtml.FragmentBuilder f) => new(f);

		// ---------------------------------------------------------------------
		// Element-level fluent wrapper
		// ---------------------------------------------------------------------

		/// <summary>
		/// Fluent wrapper around <see cref="FluentHtml.ElementBuilder"/> that applies HeimdallHtml attributes.
		/// </summary>
		public readonly struct HeimdallBuilder
		{
			private readonly FluentHtml.ElementBuilder _b;

			public HeimdallBuilder(FluentHtml.ElementBuilder builder)
				=> _b = builder ?? throw new ArgumentNullException(nameof(builder));

			// -----------------------------
			// Triggers
			// -----------------------------

			public HeimdallBuilder On(HeimdallHtml.Trigger trigger, HeimdallHtml.ActionId action)
			{
				_b.Add(HeimdallHtml.On(trigger, action));
				return this;
			}

			public HeimdallBuilder OnLoad(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnLoad(action)); return this; }
			public HeimdallBuilder OnClick(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnClick(action)); return this; }
			public HeimdallBuilder OnChange(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnChange(action)); return this; }
			public HeimdallBuilder OnInput(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnInput(action)); return this; }
			public HeimdallBuilder OnSubmit(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnSubmit(action)); return this; }
			public HeimdallBuilder OnKeyDown(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnKeyDown(action)); return this; }
			public HeimdallBuilder OnBlur(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnBlur(action)); return this; }
			public HeimdallBuilder OnHover(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnHover(action)); return this; }
			public HeimdallBuilder OnVisible(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnVisible(action)); return this; }
			public HeimdallBuilder OnScroll(HeimdallHtml.ActionId action) { _b.Add(HeimdallHtml.OnScroll(action)); return this; }

			// Short aliases (minimal-api vibe)
			public HeimdallBuilder Load(HeimdallHtml.ActionId action) => OnLoad(action);
			public HeimdallBuilder Click(HeimdallHtml.ActionId action) => OnClick(action);
			public HeimdallBuilder Change(HeimdallHtml.ActionId action) => OnChange(action);
			public HeimdallBuilder Input(HeimdallHtml.ActionId action) => OnInput(action);
			public HeimdallBuilder Submit(HeimdallHtml.ActionId action) => OnSubmit(action);
			public HeimdallBuilder KeyDown(HeimdallHtml.ActionId action) => OnKeyDown(action);
			public HeimdallBuilder Blur(HeimdallHtml.ActionId action) => OnBlur(action);
			public HeimdallBuilder Hover(HeimdallHtml.ActionId action) => OnHover(action);
			public HeimdallBuilder Visible(HeimdallHtml.ActionId action) => OnVisible(action);
			public HeimdallBuilder Scroll(HeimdallHtml.ActionId action) => OnScroll(action);

			// -----------------------------
			// Common options
			// -----------------------------

			public HeimdallBuilder Target(string selector) { _b.Add(HeimdallHtml.Target(selector)); return this; }
			public HeimdallBuilder Swap(HeimdallHtml.Swap swap) { _b.Add(HeimdallHtml.SwapMode(swap)); return this; }
			public HeimdallBuilder Disable(bool on = true) { _b.Add(HeimdallHtml.Disable(on)); return this; }
			public HeimdallBuilder PreventDefault(bool on = true) { _b.Add(HeimdallHtml.PreventDefault(on)); return this; }

			// Convenience swaps
			public HeimdallBuilder SwapInner() => Swap(HeimdallHtml.Swap.Inner);
			public HeimdallBuilder SwapOuter() => Swap(HeimdallHtml.Swap.Outer);
			public HeimdallBuilder SwapBeforeEnd() => Swap(HeimdallHtml.Swap.BeforeEnd);
			public HeimdallBuilder SwapAfterBegin() => Swap(HeimdallHtml.Swap.AfterBegin);
			public HeimdallBuilder SwapNone() => Swap(HeimdallHtml.Swap.None);

			// -----------------------------
			// Payload options
			// -----------------------------

			public HeimdallBuilder Payload(object? payload, JsonSerializerOptions? options = null)
			{
				_b.Add(HeimdallHtml.Payload(payload, options));
				return this;
			}

			public HeimdallBuilder PayloadEmptyObject() { _b.Add(HeimdallHtml.PayloadEmptyObject()); return this; }

			public HeimdallBuilder PayloadFrom(string from) { _b.Add(HeimdallHtml.PayloadFromDirective(from)); return this; }
			public HeimdallBuilder PayloadFromClosestForm() => PayloadFrom(HeimdallHtml.PayloadFrom.ClosestForm);
			public HeimdallBuilder PayloadFromSelf() => PayloadFrom(HeimdallHtml.PayloadFrom.Self);

			public HeimdallBuilder PayloadFromClosestState(string? key = null) { _b.Add(HeimdallHtml.PayloadFromClosestState(key)); return this; }

			public HeimdallBuilder PayloadRef(string globalPath) { _b.Add(HeimdallHtml.PayloadRef(globalPath)); return this; }

			// -----------------------------
			// Closest-state data attributes
			// -----------------------------

			public HeimdallBuilder State(object? state, JsonSerializerOptions? options = null) { _b.Add(HeimdallHtml.State(state, options)); return this; }
			public HeimdallBuilder State(string key, object? state, JsonSerializerOptions? options = null) { _b.Add(HeimdallHtml.State(key, state, options)); return this; }

			public HeimdallBuilder StateJson(string json) { _b.Add(HeimdallHtml.StateJson(json)); return this; }
			public HeimdallBuilder StateJson(string key, string json) { _b.Add(HeimdallHtml.StateJson(key, json)); return this; }

			// -----------------------------
			// Trigger modifiers
			// -----------------------------

			public HeimdallBuilder DebounceMs(int ms) { _b.Add(HeimdallHtml.DebounceMs(ms)); return this; }
			public HeimdallBuilder Key(string keySpec) { _b.Add(HeimdallHtml.Key(keySpec)); return this; }
			public HeimdallBuilder HoverDelayMs(int ms) { _b.Add(HeimdallHtml.HoverDelayMs(ms)); return this; }
			public HeimdallBuilder VisibleOnce(bool on = true) { _b.Add(HeimdallHtml.VisibleOnce(on)); return this; }
			public HeimdallBuilder ScrollThresholdPx(int px) { _b.Add(HeimdallHtml.ScrollThresholdPx(px)); return this; }
			public HeimdallBuilder PollMs(int ms) { _b.Add(HeimdallHtml.PollMs(ms)); return this; }

			// -----------------------------
			// SSE (Bifrost)
			// -----------------------------

			public HeimdallBuilder SseTopic(string topic) { _b.Add(HeimdallHtml.SseTopic(topic)); return this; }
			public HeimdallBuilder SseTopicAlias(string topic) { _b.Add(HeimdallHtml.SseTopicAlias(topic)); return this; }
			public HeimdallBuilder SseTarget(string selector) { _b.Add(HeimdallHtml.SseTarget(selector)); return this; }
			public HeimdallBuilder SseSwap(HeimdallHtml.Swap swap) { _b.Add(HeimdallHtml.SseSwapMode(swap)); return this; }
			public HeimdallBuilder SseEvent(string eventName) { _b.Add(HeimdallHtml.SseEvent(eventName)); return this; }
			public HeimdallBuilder SseDisable(bool on = true) { _b.Add(HeimdallHtml.SseDisable(on)); return this; }

			/// <summary>Common SSE recipe: set topic + target + swap.</summary>
			public HeimdallBuilder Sse(string topic, string targetSelector, HeimdallHtml.Swap swap = HeimdallHtml.Swap.BeforeEnd)
			{
				_b.Add(
					HeimdallHtml.SseTopic(topic),
					HeimdallHtml.SseTarget(targetSelector),
					HeimdallHtml.SseSwapMode(swap)
				);
				return this;
			}

			// -----------------------------
			// Recipes (common patterns)
			// -----------------------------

			/// <summary>Common pattern: click action with Swap.None.</summary>
			public HeimdallBuilder ClickNoSwap(HeimdallHtml.ActionId action)
				=> OnClick(action).SwapNone();

			/// <summary>Common pattern: click action with Swap.None and explicit empty JSON payload.</summary>
			public HeimdallBuilder ClickNoSwapEmptyPayload(HeimdallHtml.ActionId action)
				=> OnClick(action).SwapNone().PayloadEmptyObject();

			/// <summary>Apply trigger + target + swap in one call.</summary>
			public HeimdallBuilder Apply(HeimdallHtml.Trigger trigger, HeimdallHtml.ActionId action, string targetSelector, HeimdallHtml.Swap swap = HeimdallHtml.Swap.Inner)
			{
				_b.Add(
					HeimdallHtml.On(trigger, action),
					HeimdallHtml.Target(targetSelector),
					HeimdallHtml.SwapMode(swap)
				);
				return this;
			}

			/// <summary>Apply trigger + target + swap using an existing HeimdallHtml.Binding.</summary>
			public HeimdallBuilder Apply(HeimdallHtml.Binding binding, string targetSelector, HeimdallHtml.Swap swap = HeimdallHtml.Swap.Inner)
			{
				foreach (var a in binding.Apply(targetSelector, swap))
					_b.Add(a);
				return this;
			}
		}

		// ---------------------------------------------------------------------
		// Fragment-level fluent wrapper
		// ---------------------------------------------------------------------

		/// <summary>
		/// Fluent wrapper around <see cref="FluentHtml.FragmentBuilder"/> for Heimdall-centric fragment content.
		/// </summary>
		public readonly struct HeimdallFragmentBuilder
		{
			private readonly FluentHtml.FragmentBuilder _f;

			public HeimdallFragmentBuilder(FluentHtml.FragmentBuilder fragment)
				=> _f = fragment ?? throw new ArgumentNullException(nameof(fragment));

			/// <summary>Adds an &lt;invocation&gt; element for OOB swaps.</summary>
			public HeimdallFragmentBuilder Invocation(
				string targetSelector,
				HeimdallHtml.Swap swap = HeimdallHtml.Swap.Inner,
				IHtmlContent? payload = null,
				bool wrapInTemplate = false)
			{
				_f.Add(HeimdallHtml.Invocation(targetSelector, swap, payload, wrapInTemplate));
				return this;
			}

			/// <summary>Adds any IHtmlContent to the fragment.</summary>
			public HeimdallFragmentBuilder Add(IHtmlContent content)
			{
				_f.Add(content);
				return this;
			}
		}
	}
}
