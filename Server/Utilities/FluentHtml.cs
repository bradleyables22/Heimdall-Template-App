using Microsoft.AspNetCore.Html;
using System.Buffers;

namespace HeimdallTemplateApp.Utilities
{
    /// <summary>
    /// Fluent / block-style HTML builder that mirrors the ergonomics of <see cref="Html"/> while enabling
    /// normal C# control flow (if/foreach/switch/locals) when constructing markup.
    ///
    /// Design goals:
    /// - <see cref="Html"/> remains the single rendering/encoding/merge core.
    /// - <see cref="FluentHtml"/> provides a lambda/builder layer that collects "parts" (attrs + children)
    ///   and delegates final rendering to <see cref="Html.Tag(string, object?[])"/> / <see cref="Html.VoidTag(string, object?[])"/>.
    /// - Keep the fluent layer allocation-light (ArrayPool-backed buffers) and easy to read.
    ///
    /// Usage example:
    /// <code>
    /// var content = FluentHtml.Div(d =>
    /// {
    ///     d.Class("card");
    ///     d.Label(l => l.For("Email").Text("Email"));
    ///     d.Input(Html.InputType.email, i => i.Id("Email").Class("form-control"));
    ///     d.TextArea(t => t.Name("Notes").Class("form-control").Text("Hello"));
    /// });
    /// </code>
    /// </summary>
    public static class FluentHtml
    {
        // ============================================================
        // Element factories (mirror Html)
        // ============================================================

        /// <summary>
        /// Builds a non-void element using a builder block (e.g. &lt;div&gt;...&lt;/div&gt;).
        /// The builder collects attributes and children and delegates final rendering to <see cref="Html.Tag"/>.
        /// </summary>
        public static IHtmlContent Tag(string name, Action<ElementBuilder> build)
            => BuildTag(name, isVoid: false, build);

        /// <summary>
        /// Builds a void element using a builder block (e.g. &lt;input /&gt;).
        /// The builder collects attributes and (ignored) children and delegates final rendering to <see cref="Html.VoidTag"/>.
        /// </summary>
        public static IHtmlContent VoidTag(string name, Action<ElementBuilder> build)
            => BuildTag(name, isVoid: true, build);

        /// <summary>
        /// Mirrors <see cref="Html.Tag(string, object?[])"/> for non-builder call sites, enabling FluentHtml
        /// to be a drop-in replacement when you don't want lambdas.
        /// </summary>
        public static IHtmlContent Tag(string name, params object?[] parts)
            => Html.Tag(name, parts);

        /// <summary>
        /// Mirrors <see cref="Html.VoidTag(string, object?[])"/> for non-builder call sites.
        /// </summary>
        public static IHtmlContent VoidTag(string name, params object?[] parts)
            => Html.VoidTag(name, parts);

        /// <summary>
        /// Builds a fragment using a builder block (no wrapper element).
        /// Useful when you want to emit a sequence of nodes with normal C# flow.
        /// </summary>
        public static IHtmlContent Fragment(Action<FragmentBuilder> build)
        {
            using var fb = new FragmentBuilder(initialCapacity: 8);
            build(fb);
            return Html.Fragment(fb.ToArray());
        }

        /// <summary>
        /// Mirrors <see cref="Html.Fragment(object?[])"/> for non-builder call sites.
        /// </summary>
        public static IHtmlContent Fragment(params object?[] parts)
            => Html.Fragment(parts);

        /// <summary>
        /// Mirrors <see cref="Html.Text(string?)"/> (encoded text).
        /// </summary>
        public static IHtmlContent Text(string? text) => Html.Text(text);

        /// <summary>
        /// Mirrors <see cref="Html.Raw(string?)"/> (unencoded HTML).
        /// </summary>
        public static IHtmlContent Raw(string? html) => Html.Raw(html);

        // ============================================================
        // Attribute helpers (mirror Html)
        // ============================================================

        /// <summary>Creates a normal attribute name="value".</summary>
        public static Html.HtmlAttr Attr(string name, string? value) => Html.Attr(name, value);

        /// <summary>
        /// Creates a label 'for' attribute. In HTML this should match the target element's id.
        /// </summary>
        public static Html.HtmlAttr For(string value) => Html.For(value);

        /// <summary>Creates a boolean attribute (presence-only when on == true).</summary>
        public static Html.HtmlAttr Bool(string name, bool on) => Html.Bool(name, on);

        /// <summary>Creates a class attribute; multiple class attrs on the same element will be merged.</summary>
        public static Html.HtmlAttr Class(params string?[] classes) => Html.Class(classes);

        /// <summary>Creates id="...".</summary>
        public static Html.HtmlAttr Id(string id) => Html.Id(id);

        /// <summary>Creates href="...".</summary>
        public static Html.HtmlAttr Href(string href) => Html.Href(href);

        /// <summary>Creates src="...".</summary>
        public static Html.HtmlAttr Src(string src) => Html.Src(src);

        /// <summary>Creates alt="...".</summary>
        public static Html.HtmlAttr Alt(string alt) => Html.Alt(alt);

        /// <summary>Creates type="..." (string form).</summary>
        public static Html.HtmlAttr Type(string type) => Html.Type(type);

        /// <summary>Creates type="..." from <see cref="Html.InputType"/> (enum form).</summary>
        public static Html.HtmlAttr Type(Html.InputType type) => Html.Type(type);

        /// <summary>Creates name="...".</summary>
        public static Html.HtmlAttr Name(string name) => Html.Name(name);

        /// <summary>Creates value="...".</summary>
        public static Html.HtmlAttr Value(string value) => Html.Value(value);

        /// <summary>Creates role="...".</summary>
        public static Html.HtmlAttr Role(string role) => Html.Role(role);

        /// <summary>Creates style="...".</summary>
        public static Html.HtmlAttr Style(string css) => Html.Style(css);

        /// <summary>
        /// Creates content="..." (commonly used on meta tags).
        /// Named ContentAttr to avoid colliding with builder methods like Content(IHtmlContent).
        /// </summary>
        public static Html.HtmlAttr ContentAttr(string value) => Html.Content(value);

        /// <summary>Creates data-{key}="value".</summary>
        public static Html.HtmlAttr Data(string key, string value) => Html.Data(key, value);

        /// <summary>Creates aria-{key}="value".</summary>
        public static Html.HtmlAttr Aria(string key, string value) => Html.Aria(key, value);

        /// <summary>Creates disabled attribute (presence-only).</summary>
        public static Html.HtmlAttr Disabled(bool on = true) => Html.Disabled(on);

        /// <summary>Creates checked attribute (presence-only).</summary>
        public static Html.HtmlAttr Checked(bool on = true) => Html.Checked(on);

        /// <summary>Creates selected attribute (presence-only).</summary>
        public static Html.HtmlAttr Selected(bool on = true) => Html.Selected(on);

        /// <summary>Creates readonly attribute (presence-only).</summary>
        public static Html.HtmlAttr ReadOnly(bool on = true) => Html.ReadOnly(on);

        /// <summary>Creates required attribute (presence-only).</summary>
        public static Html.HtmlAttr Required(bool on = true) => Html.Required(on);

        // ============================================================
        // Common tags (ergonomic wrappers) - mirror Html
        // ============================================================

        // Non-void
        public static IHtmlContent Title(Action<ElementBuilder> b) => Tag("title", b);
        public static IHtmlContent Div(Action<ElementBuilder> b) => Tag("div", b);
        public static IHtmlContent Span(Action<ElementBuilder> b) => Tag("span", b);
        public static IHtmlContent Strong(Action<ElementBuilder> b) => Tag("strong", b);
        public static IHtmlContent P(Action<ElementBuilder> b) => Tag("p", b);
        public static IHtmlContent H1(Action<ElementBuilder> b) => Tag("h1", b);
        public static IHtmlContent H2(Action<ElementBuilder> b) => Tag("h2", b);
        public static IHtmlContent H3(Action<ElementBuilder> b) => Tag("h3", b);
        public static IHtmlContent H4(Action<ElementBuilder> b) => Tag("h4", b);
        public static IHtmlContent H5(Action<ElementBuilder> b) => Tag("h5", b);
        public static IHtmlContent H6(Action<ElementBuilder> b) => Tag("h6", b);
        public static IHtmlContent Ul(Action<ElementBuilder> b) => Tag("ul", b);
        public static IHtmlContent Li(Action<ElementBuilder> b) => Tag("li", b);
        public static IHtmlContent A(Action<ElementBuilder> b) => Tag("a", b);
        public static IHtmlContent Button(Action<ElementBuilder> b) => Tag("button", b);
        public static IHtmlContent Nav(Action<ElementBuilder> b) => Tag("nav", b);
        public static IHtmlContent Form(Action<ElementBuilder> b) => Tag("form", b);
        public static IHtmlContent Label(Action<ElementBuilder> b) => Tag("label", b);
        public static IHtmlContent I(Action<ElementBuilder> b) => Tag("i", b);
        public static IHtmlContent Code(Action<ElementBuilder> b) => Tag("code", b);
        public static IHtmlContent Pre(Action<ElementBuilder> b) => Tag("pre", b);

        /// <summary>
        /// FIX: textarea is NOT an input type. It is its own tag.
        /// </summary>
        public static IHtmlContent TextArea(Action<ElementBuilder> b) => Tag("textarea", b);

        /// <summary>Optional alias if you prefer the HTML casing.</summary>
        public static IHtmlContent Textarea(Action<ElementBuilder> b) => Tag("textarea", b);

        public static IHtmlContent CodeBlock(string language, Action<ElementBuilder> build)
            => Tag("pre", p =>
            {
                p.Tag("code", c =>
                {
                    c.Class($"language-{language}");
                    build(c);
                });
            });

        // Table tags
        public static IHtmlContent TableHead(Action<ElementBuilder> b) => Tag("thead", b);
        public static IHtmlContent TableBody(Action<ElementBuilder> b) => Tag("tbody", b);
        public static IHtmlContent TableFoot(Action<ElementBuilder> b) => Tag("tfoot", b);
        public static IHtmlContent TableRow(Action<ElementBuilder> b) => Tag("tr", b);
        public static IHtmlContent TableHeaderCell(Action<ElementBuilder> b) => Tag("th", b);
        public static IHtmlContent TableDataCell(Action<ElementBuilder> b) => Tag("td", b);
        public static IHtmlContent Caption(Action<ElementBuilder> b) => Tag("caption", b);

        public static IHtmlContent Script(Action<ElementBuilder> b) => Tag("script", b);
        public static IHtmlContent NoScript(Action<ElementBuilder> b) => Tag("noscript", b);
        public static IHtmlContent Template(Action<ElementBuilder> b) => Tag("template", b);

        // Void tags
        public static IHtmlContent Br(Action<ElementBuilder> b) => VoidTag("br", b);
        public static IHtmlContent Hr(Action<ElementBuilder> b) => VoidTag("hr", b);
        public static IHtmlContent Img(Action<ElementBuilder> b) => VoidTag("img", b);

        /// <summary>Creates &lt;input /&gt; using a builder for attributes (children ignored).</summary>
        public static IHtmlContent Input(Action<ElementBuilder> b) => VoidTag("input", b);

        /// <summary>
        /// Creates &lt;input type="{type}" /&gt; using a builder for additional attributes (children ignored).
        /// IMPORTANT: Html.InputType should NOT contain 'textarea'.
        /// </summary>
        public static IHtmlContent Input(Html.InputType type, Action<ElementBuilder> build)
            => VoidTag("input", b =>
            {
                b.Type(type);
                build(b);
            });

        /// <summary>
        /// Mirrors <see cref="Html.Input(Html.InputType, object?[])"/> for non-builder call sites.
        /// </summary>
        public static IHtmlContent Input(Html.InputType type, params object?[] parts)
            => Html.Input(type, parts);

        public static IHtmlContent Meta(Action<ElementBuilder> b) => VoidTag("meta", b);
        public static IHtmlContent Link(Action<ElementBuilder> b) => VoidTag("link", b);

        // Root-ish tags
        public static IHtmlContent HtmlTag(Action<ElementBuilder> b) => Tag("html", b);
        public static IHtmlContent Head(Action<ElementBuilder> b) => Tag("head", b);
        public static IHtmlContent Body(Action<ElementBuilder> b) => Tag("body", b);
        public static IHtmlContent Footer(Action<ElementBuilder> b) => Tag("footer", b);

        // ============================================================
        // Builders
        // ============================================================

        public sealed class ElementBuilder : IDisposable
        {
            private PooledBuffer<object?> _parts;

            internal ElementBuilder(int initialCapacity)
            {
                _parts.Init(initialCapacity);
            }

            internal object?[] ToArray() => _parts.ToArray();
            public void Dispose() => _parts.Dispose();

            // ----------------------------
            // Core add/content operations
            // ----------------------------

            public ElementBuilder Add(object? part)
            {
                _parts.Add(part);
                return this;
            }

            public ElementBuilder Add(params object?[] parts)
            {
                if (parts is null || parts.Length == 0) return this;
                for (int i = 0; i < parts.Length; i++)
                    _parts.Add(parts[i]);
                return this;
            }

            public ElementBuilder Content(IHtmlContent content)
            {
                _parts.Add(content);
                return this;
            }

            public ElementBuilder Text(string? text)
            {
                _parts.Add(Html.Text(text));
                return this;
            }

            public ElementBuilder Raw(string? html)
            {
                _parts.Add(Html.Raw(html));
                return this;
            }

            // ----------------------------
            // Attribute helpers (chainable) - mirror Html
            // ----------------------------

            public ElementBuilder Attr(string name, string? value) { _parts.Add(Html.Attr(name, value)); return this; }
            public ElementBuilder For(string value) { _parts.Add(Html.For(value)); return this; }
            public ElementBuilder Bool(string name, bool on) { _parts.Add(Html.Bool(name, on)); return this; }
            public ElementBuilder Class(params string?[] classes) { _parts.Add(Html.Class(classes)); return this; }
            public ElementBuilder Id(string id) { _parts.Add(Html.Id(id)); return this; }
            public ElementBuilder Href(string href) { _parts.Add(Html.Href(href)); return this; }
            public ElementBuilder Src(string src) { _parts.Add(Html.Src(src)); return this; }
            public ElementBuilder Alt(string alt) { _parts.Add(Html.Alt(alt)); return this; }
            public ElementBuilder Type(string type) { _parts.Add(Html.Type(type)); return this; }
            public ElementBuilder Type(Html.InputType type) { _parts.Add(Html.Type(type)); return this; }
            public ElementBuilder Name(string name) { _parts.Add(Html.Name(name)); return this; }
            public ElementBuilder Value(string value) { _parts.Add(Html.Value(value)); return this; }
            public ElementBuilder Role(string role) { _parts.Add(Html.Role(role)); return this; }
            public ElementBuilder Style(string css) { _parts.Add(Html.Style(css)); return this; }
            public ElementBuilder ContentAttr(string value) { _parts.Add(Html.Content(value)); return this; }
            public ElementBuilder Data(string key, string value) { _parts.Add(Html.Data(key, value)); return this; }
            public ElementBuilder Aria(string key, string value) { _parts.Add(Html.Aria(key, value)); return this; }
            public ElementBuilder Disabled(bool on = true) { _parts.Add(Html.Disabled(on)); return this; }
            public ElementBuilder Checked(bool on = true) { _parts.Add(Html.Checked(on)); return this; }
            public ElementBuilder Selected(bool on = true) { _parts.Add(Html.Selected(on)); return this; }
            public ElementBuilder ReadOnly(bool on = true) { _parts.Add(Html.ReadOnly(on)); return this; }
            public ElementBuilder Required(bool on = true) { _parts.Add(Html.Required(on)); return this; }

            // ----------------------------
            // Nested tags (adds as children) - mirror FluentHtml
            // ----------------------------

            public ElementBuilder Tag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.Tag(name, build)); return this; }
            public ElementBuilder VoidTag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.VoidTag(name, build)); return this; }

            public ElementBuilder Title(Action<ElementBuilder> b) => Tag("title", b);
            public ElementBuilder Div(Action<ElementBuilder> b) => Tag("div", b);
            public ElementBuilder Span(Action<ElementBuilder> b) => Tag("span", b);
            public ElementBuilder Strong(Action<ElementBuilder> b) => Tag("strong", b);
            public ElementBuilder P(Action<ElementBuilder> b) => Tag("p", b);
            public ElementBuilder H1(Action<ElementBuilder> b) => Tag("h1", b);
            public ElementBuilder H2(Action<ElementBuilder> b) => Tag("h2", b);
            public ElementBuilder H3(Action<ElementBuilder> b) => Tag("h3", b);
            public ElementBuilder H4(Action<ElementBuilder> b) => Tag("h4", b);
            public ElementBuilder H5(Action<ElementBuilder> b) => Tag("h5", b);
            public ElementBuilder H6(Action<ElementBuilder> b) => Tag("h6", b);
            public ElementBuilder Ul(Action<ElementBuilder> b) => Tag("ul", b);
            public ElementBuilder Li(Action<ElementBuilder> b) => Tag("li", b);
            public ElementBuilder A(Action<ElementBuilder> b) => Tag("a", b);
            public ElementBuilder Button(Action<ElementBuilder> b) => Tag("button", b);
            public ElementBuilder Nav(Action<ElementBuilder> b) => Tag("nav", b);
            public ElementBuilder Form(Action<ElementBuilder> b) => Tag("form", b);
            public ElementBuilder Label(Action<ElementBuilder> b) => Tag("label", b);
            public ElementBuilder I(Action<ElementBuilder> b) => Tag("i", b);
            public ElementBuilder Code(Action<ElementBuilder> b) => Tag("code", b);
            public ElementBuilder Pre(Action<ElementBuilder> b) => Tag("pre", b);

            /// <summary>
            /// FIX: textarea is its own element, not &lt;input type="..."&gt;.
            /// </summary>
            public ElementBuilder TextArea(Action<ElementBuilder> b) => Tag("textarea", b);

            /// <summary>Optional alias.</summary>
            public ElementBuilder Textarea(Action<ElementBuilder> b) => Tag("textarea", b);

            public ElementBuilder CodeBlock(string language, Action<ElementBuilder> build)
            {
                _parts.Add(FluentHtml.CodeBlock(language, build));
                return this;
            }

            public ElementBuilder TableHead(Action<ElementBuilder> b) => Tag("thead", b);
            public ElementBuilder TableBody(Action<ElementBuilder> b) => Tag("tbody", b);
            public ElementBuilder TableFoot(Action<ElementBuilder> b) => Tag("tfoot", b);
            public ElementBuilder TableRow(Action<ElementBuilder> b) => Tag("tr", b);
            public ElementBuilder TableHeaderCell(Action<ElementBuilder> b) => Tag("th", b);
            public ElementBuilder TableDataCell(Action<ElementBuilder> b) => Tag("td", b);
            public ElementBuilder Caption(Action<ElementBuilder> b) => Tag("caption", b);

            public ElementBuilder Script(Action<ElementBuilder> b) => Tag("script", b);
            public ElementBuilder NoScript(Action<ElementBuilder> b) => Tag("noscript", b);
            public ElementBuilder Template(Action<ElementBuilder> b) => Tag("template", b);

            public ElementBuilder Br(Action<ElementBuilder> b) => VoidTag("br", b);
            public ElementBuilder Hr(Action<ElementBuilder> b) => VoidTag("hr", b);
            public ElementBuilder Img(Action<ElementBuilder> b) => VoidTag("img", b);
            public ElementBuilder Input(Action<ElementBuilder> b) => VoidTag("input", b);

            public ElementBuilder Input(Html.InputType type, Action<ElementBuilder> build)
            {
                _parts.Add(FluentHtml.Input(type, build));
                return this;
            }

            public ElementBuilder Meta(Action<ElementBuilder> b) => VoidTag("meta", b);
            public ElementBuilder Link(Action<ElementBuilder> b) => VoidTag("link", b);

            public ElementBuilder HtmlTag(Action<ElementBuilder> b) => Tag("html", b);
            public ElementBuilder Head(Action<ElementBuilder> b) => Tag("head", b);
            public ElementBuilder Body(Action<ElementBuilder> b) => Tag("body", b);
            public ElementBuilder Footer(Action<ElementBuilder> b) => Tag("footer", b);
        }

        public sealed class FragmentBuilder : IDisposable
        {
            private PooledBuffer<object?> _parts;

            internal FragmentBuilder(int initialCapacity)
            {
                _parts.Init(initialCapacity);
            }

            internal object?[] ToArray() => _parts.ToArray();
            public void Dispose() => _parts.Dispose();

            public FragmentBuilder Add(object? part) { _parts.Add(part); return this; }

            public FragmentBuilder Add(params object?[] parts)
            {
                if (parts is null || parts.Length == 0) return this;
                for (int i = 0; i < parts.Length; i++)
                    _parts.Add(parts[i]);
                return this;
            }

            public FragmentBuilder Content(IHtmlContent content) { _parts.Add(content); return this; }
            public FragmentBuilder Text(string? text) { _parts.Add(Html.Text(text)); return this; }
            public FragmentBuilder Raw(string? html) { _parts.Add(Html.Raw(html)); return this; }

            // Attribute helpers (optional symmetry)
            public FragmentBuilder For(string value) { _parts.Add(Html.For(value)); return this; }
            public FragmentBuilder Type(Html.InputType type) { _parts.Add(Html.Type(type)); return this; }
            public FragmentBuilder Type(string type) { _parts.Add(Html.Type(type)); return this; }
            public FragmentBuilder Class(params string?[] classes) { _parts.Add(Html.Class(classes)); return this; }
            public FragmentBuilder Id(string id) { _parts.Add(Html.Id(id)); return this; }
            public FragmentBuilder Name(string name) { _parts.Add(Html.Name(name)); return this; }
            public FragmentBuilder Value(string value) { _parts.Add(Html.Value(value)); return this; }
            public FragmentBuilder Bool(string name, bool on) { _parts.Add(Html.Bool(name, on)); return this; }
            public FragmentBuilder Disabled(bool on = true) { _parts.Add(Html.Disabled(on)); return this; }
            public FragmentBuilder Checked(bool on = true) { _parts.Add(Html.Checked(on)); return this; }
            public FragmentBuilder Selected(bool on = true) { _parts.Add(Html.Selected(on)); return this; }
            public FragmentBuilder ReadOnly(bool on = true) { _parts.Add(Html.ReadOnly(on)); return this; }
            public FragmentBuilder Required(bool on = true) { _parts.Add(Html.Required(on)); return this; }
            public FragmentBuilder Data(string key, string value) { _parts.Add(Html.Data(key, value)); return this; }
            public FragmentBuilder Aria(string key, string value) { _parts.Add(Html.Aria(key, value)); return this; }
            public FragmentBuilder Style(string css) { _parts.Add(Html.Style(css)); return this; }
            public FragmentBuilder Href(string href) { _parts.Add(Html.Href(href)); return this; }
            public FragmentBuilder Src(string src) { _parts.Add(Html.Src(src)); return this; }
            public FragmentBuilder Alt(string alt) { _parts.Add(Html.Alt(alt)); return this; }
            public FragmentBuilder Role(string role) { _parts.Add(Html.Role(role)); return this; }
            public FragmentBuilder ContentAttr(string value) { _parts.Add(Html.Content(value)); return this; }
            public FragmentBuilder Attr(string name, string? value) { _parts.Add(Html.Attr(name, value)); return this; }

            // Generic tag helpers
            public FragmentBuilder Tag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.Tag(name, build)); return this; }
            public FragmentBuilder VoidTag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.VoidTag(name, build)); return this; }

            // Mirror ElementBuilder wrappers (non-void)
            public FragmentBuilder Title(Action<ElementBuilder> b) => Tag("title", b);
            public FragmentBuilder Div(Action<ElementBuilder> b) => Tag("div", b);
            public FragmentBuilder Span(Action<ElementBuilder> b) => Tag("span", b);
            public FragmentBuilder Strong(Action<ElementBuilder> b) => Tag("strong", b);
            public FragmentBuilder P(Action<ElementBuilder> b) => Tag("p", b);
            public FragmentBuilder H1(Action<ElementBuilder> b) => Tag("h1", b);
            public FragmentBuilder H2(Action<ElementBuilder> b) => Tag("h2", b);
            public FragmentBuilder H3(Action<ElementBuilder> b) => Tag("h3", b);
            public FragmentBuilder H4(Action<ElementBuilder> b) => Tag("h4", b);
            public FragmentBuilder H5(Action<ElementBuilder> b) => Tag("h5", b);
            public FragmentBuilder H6(Action<ElementBuilder> b) => Tag("h6", b);
            public FragmentBuilder Ul(Action<ElementBuilder> b) => Tag("ul", b);
            public FragmentBuilder Li(Action<ElementBuilder> b) => Tag("li", b);
            public FragmentBuilder A(Action<ElementBuilder> b) => Tag("a", b);
            public FragmentBuilder Button(Action<ElementBuilder> b) => Tag("button", b);
            public FragmentBuilder Nav(Action<ElementBuilder> b) => Tag("nav", b);
            public FragmentBuilder Form(Action<ElementBuilder> b) => Tag("form", b);
            public FragmentBuilder Label(Action<ElementBuilder> b) => Tag("label", b);
            public FragmentBuilder I(Action<ElementBuilder> b) => Tag("i", b);
            public FragmentBuilder Code(Action<ElementBuilder> b) => Tag("code", b);
            public FragmentBuilder Pre(Action<ElementBuilder> b) => Tag("pre", b);

            /// <summary>
            /// FIX: textarea is its own element.
            /// </summary>
            public FragmentBuilder TextArea(Action<ElementBuilder> b) => Tag("textarea", b);

            /// <summary>Optional alias.</summary>
            public FragmentBuilder Textarea(Action<ElementBuilder> b) => Tag("textarea", b);

            public FragmentBuilder CodeBlock(string language, Action<ElementBuilder> build)
            {
                _parts.Add(FluentHtml.CodeBlock(language, build));
                return this;
            }

            // Table wrappers
            public FragmentBuilder TableHead(Action<ElementBuilder> b) => Tag("thead", b);
            public FragmentBuilder TableBody(Action<ElementBuilder> b) => Tag("tbody", b);
            public FragmentBuilder TableFoot(Action<ElementBuilder> b) => Tag("tfoot", b);
            public FragmentBuilder TableRow(Action<ElementBuilder> b) => Tag("tr", b);
            public FragmentBuilder TableHeaderCell(Action<ElementBuilder> b) => Tag("th", b);
            public FragmentBuilder TableDataCell(Action<ElementBuilder> b) => Tag("td", b);
            public FragmentBuilder Caption(Action<ElementBuilder> b) => Tag("caption", b);

            public FragmentBuilder Script(Action<ElementBuilder> b) => Tag("script", b);
            public FragmentBuilder NoScript(Action<ElementBuilder> b) => Tag("noscript", b);
            public FragmentBuilder Template(Action<ElementBuilder> b) => Tag("template", b);

            // Void wrappers
            public FragmentBuilder Br(Action<ElementBuilder> b) => VoidTag("br", b);
            public FragmentBuilder Hr(Action<ElementBuilder> b) => VoidTag("hr", b);
            public FragmentBuilder Img(Action<ElementBuilder> b) => VoidTag("img", b);
            public FragmentBuilder Input(Action<ElementBuilder> b) => VoidTag("input", b);

            public FragmentBuilder Input(Html.InputType type, Action<ElementBuilder> build)
            {
                _parts.Add(FluentHtml.Input(type, build));
                return this;
            }

            public FragmentBuilder Meta(Action<ElementBuilder> b) => VoidTag("meta", b);
            public FragmentBuilder Link(Action<ElementBuilder> b) => VoidTag("link", b);

            // Root-ish wrappers
            public FragmentBuilder HtmlTag(Action<ElementBuilder> b) => Tag("html", b);
            public FragmentBuilder Head(Action<ElementBuilder> b) => Tag("head", b);
            public FragmentBuilder Body(Action<ElementBuilder> b) => Tag("body", b);
            public FragmentBuilder Footer(Action<ElementBuilder> b) => Tag("footer", b);
        }

        // ============================================================
        // Internals
        // ============================================================

        private static IHtmlContent BuildTag(string name, bool isVoid, Action<ElementBuilder> build)
        {
            using var b = new ElementBuilder(initialCapacity: 10);
            build(b);

            var parts = b.ToArray();
            return isVoid ? Html.VoidTag(name, parts) : Html.Tag(name, parts);
        }

        private struct PooledBuffer<T>
        {
            private T[]? _arr;
            private int _count;

            public void Init(int initialCapacity = 8)
            {
                _arr = ArrayPool<T>.Shared.Rent(initialCapacity);
                _count = 0;
            }

            public void Add(T item)
            {
                if (_arr is null)
                    Init();

                if (_count == _arr!.Length)
                {
                    var old = _arr!;
                    _arr = ArrayPool<T>.Shared.Rent(old.Length * 2);
                    Array.Copy(old, 0, _arr, 0, old.Length);
                    ArrayPool<T>.Shared.Return(old, clearArray: true);
                }

                _arr![_count++] = item;
            }

            public T[] ToArray()
            {
                if (_arr is null || _count == 0)
                    return Array.Empty<T>();

                var result = new T[_count];
                Array.Copy(_arr, 0, result, 0, _count);
                return result;
            }

            public void Dispose()
            {
                if (_arr is not null)
                {
                    ArrayPool<T>.Shared.Return(_arr, clearArray: true);
                    _arr = null;
                    _count = 0;
                }
            }
        }
    }
}
