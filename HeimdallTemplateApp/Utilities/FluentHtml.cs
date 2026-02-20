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
    /// </summary>
    public static class FluentHtml
    {
        // ============================================================
        // Element factories (mirror Html)
        // ============================================================

        public static IHtmlContent Tag(string name, Action<ElementBuilder> build)
            => BuildTag(name, isVoid: false, build);

        public static IHtmlContent VoidTag(string name, Action<ElementBuilder> build)
            => BuildTag(name, isVoid: true, build);

        public static IHtmlContent Tag(string name, params object?[] parts)
            => Html.Tag(name, parts);

        public static IHtmlContent VoidTag(string name, params object?[] parts)
            => Html.VoidTag(name, parts);

        public static IHtmlContent Fragment(Action<FragmentBuilder> build)
        {
            using var fb = new FragmentBuilder(initialCapacity: 8);
            build(fb);
            return Html.Fragment(fb.ToArray());
        }

        public static IHtmlContent Fragment(params object?[] parts)
            => Html.Fragment(parts);

        public static IHtmlContent Text(string? text) => Html.Text(text);
        public static IHtmlContent Raw(string? html) => Html.Raw(html);

        // ============================================================
        // Attribute helpers (mirror Html)
        // ============================================================

        public static Html.HtmlAttr Attr(string name, string? value) => Html.Attr(name, value);
        public static Html.HtmlAttr For(string value) => Html.For(value);
        public static Html.HtmlAttr Bool(string name, bool on) => Html.Bool(name, on);
        public static Html.HtmlAttr Class(params string?[] classes) => Html.Class(classes);
        public static Html.HtmlAttr Id(string id) => Html.Id(id);
        public static Html.HtmlAttr Href(string href) => Html.Href(href);
        public static Html.HtmlAttr Src(string src) => Html.Src(src);
        public static Html.HtmlAttr Alt(string alt) => Html.Alt(alt);
        public static Html.HtmlAttr Type(string type) => Html.Type(type);
        public static Html.HtmlAttr Type(Html.InputType type) => Html.Type(type);
        public static Html.HtmlAttr Name(string name) => Html.Name(name);
        public static Html.HtmlAttr Value(string value) => Html.Value(value);
        public static Html.HtmlAttr Role(string role) => Html.Role(role);
        public static Html.HtmlAttr Style(string css) => Html.Style(css);
        public static Html.HtmlAttr ContentAttr(string value) => Html.Content(value);
        public static Html.HtmlAttr TitleAttr(string value) => Html.TitleAttr(value);
        public static Html.HtmlAttr Data(string key, string value) => Html.Data(key, value);
        public static Html.HtmlAttr Aria(string key, string value) => Html.Aria(key, value);

        public static Html.HtmlAttr Placeholder(string value) => Html.Placeholder(value);
        public static Html.HtmlAttr AutoComplete(string value) => Html.AutoComplete(value);
        public static Html.HtmlAttr Min(string value) => Html.Min(value);
        public static Html.HtmlAttr Max(string value) => Html.Max(value);
        public static Html.HtmlAttr Step(string value) => Html.Step(value);
        public static Html.HtmlAttr Pattern(string value) => Html.Pattern(value);
        public static Html.HtmlAttr MaxLength(int value) => Html.MaxLength(value);
        public static Html.HtmlAttr MinLength(int value) => Html.MinLength(value);
        public static Html.HtmlAttr Rows(int value) => Html.Rows(value);
        public static Html.HtmlAttr Cols(int value) => Html.Cols(value);
        public static Html.HtmlAttr Action(string value) => Html.Action(value);
        public static Html.HtmlAttr Method(string value) => Html.Method(value);
        public static Html.HtmlAttr EncType(string value) => Html.EncType(value);
        public static Html.HtmlAttr Rel(string value) => Html.Rel(value);
        public static Html.HtmlAttr Target(string value) => Html.Target(value);

        public static Html.HtmlAttr Disabled(bool on = true) => Html.Disabled(on);
        public static Html.HtmlAttr Checked(bool on = true) => Html.Checked(on);
        public static Html.HtmlAttr Selected(bool on = true) => Html.Selected(on);
        public static Html.HtmlAttr ReadOnly(bool on = true) => Html.ReadOnly(on);
        public static Html.HtmlAttr Required(bool on = true) => Html.Required(on);
        public static Html.HtmlAttr Multiple(bool on = true) => Html.Multiple(on);
        public static Html.HtmlAttr AutoFocus(bool on = true) => Html.AutoFocus(on);

        // ============================================================
        // Common tags (ergonomic wrappers) - mirror Html
        // ============================================================

        // Structure / head
        public static IHtmlContent HtmlTag(Action<ElementBuilder> b) => Tag("html", b);
        public static IHtmlContent Head(Action<ElementBuilder> b) => Tag("head", b);
        public static IHtmlContent Body(Action<ElementBuilder> b) => Tag("body", b);
        public static IHtmlContent Header(Action<ElementBuilder> b) => Tag("header", b);
        public static IHtmlContent Main(Action<ElementBuilder> b) => Tag("main", b);
        public static IHtmlContent Section(Action<ElementBuilder> b) => Tag("section", b);
        public static IHtmlContent Article(Action<ElementBuilder> b) => Tag("article", b);
        public static IHtmlContent Aside(Action<ElementBuilder> b) => Tag("aside", b);
        public static IHtmlContent Footer(Action<ElementBuilder> b) => Tag("footer", b);
        public static IHtmlContent Nav(Action<ElementBuilder> b) => Tag("nav", b);

        public static IHtmlContent Title(Action<ElementBuilder> b) => Tag("title", b);
        public static IHtmlContent Script(Action<ElementBuilder> b) => Tag("script", b);
        public static IHtmlContent NoScript(Action<ElementBuilder> b) => Tag("noscript", b);
        public static IHtmlContent Meta(Action<ElementBuilder> b) => VoidTag("meta", b);
        public static IHtmlContent Link(Action<ElementBuilder> b) => VoidTag("link", b);

        // Content
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
        public static IHtmlContent I(Action<ElementBuilder> b) => Tag("i", b);
        public static IHtmlContent Code(Action<ElementBuilder> b) => Tag("code", b);
        public static IHtmlContent Pre(Action<ElementBuilder> b) => Tag("pre", b);
        public static IHtmlContent Template(Action<ElementBuilder> b) => Tag("template", b);

        // Figure
        public static IHtmlContent Figure(Action<ElementBuilder> b) => Tag("figure", b);
        public static IHtmlContent FigCaption(Action<ElementBuilder> b) => Tag("figcaption", b);

        // Forms
        public static IHtmlContent Form(Action<ElementBuilder> b) => Tag("form", b);
        public static IHtmlContent Label(Action<ElementBuilder> b) => Tag("label", b);
        public static IHtmlContent TextArea(Action<ElementBuilder> b) => Tag("textarea", b);
        public static IHtmlContent Textarea(Action<ElementBuilder> b) => Tag("textarea", b); // alias
        public static IHtmlContent Fieldset(Action<ElementBuilder> b) => Tag("fieldset", b);
        public static IHtmlContent Legend(Action<ElementBuilder> b) => Tag("legend", b);

        // Selects
        public static IHtmlContent Select(Action<ElementBuilder> b) => Tag("select", b);
        public static IHtmlContent Option(Action<ElementBuilder> b) => Tag("option", b);
        public static IHtmlContent OptGroup(Action<ElementBuilder> b) => Tag("optgroup", b);
        public static IHtmlContent DataList(Action<ElementBuilder> b) => Tag("datalist", b);

        // Native disclosure / dialogs
        public static IHtmlContent Details(Action<ElementBuilder> b) => Tag("details", b);
        public static IHtmlContent Summary(Action<ElementBuilder> b) => Tag("summary", b);
        public static IHtmlContent Dialog(Action<ElementBuilder> b) => Tag("dialog", b);

        // Tables
        public static IHtmlContent TableHead(Action<ElementBuilder> b) => Tag("thead", b);
        public static IHtmlContent TableBody(Action<ElementBuilder> b) => Tag("tbody", b);
        public static IHtmlContent TableFoot(Action<ElementBuilder> b) => Tag("tfoot", b);
        public static IHtmlContent TableRow(Action<ElementBuilder> b) => Tag("tr", b);
        public static IHtmlContent TableHeaderCell(Action<ElementBuilder> b) => Tag("th", b);
        public static IHtmlContent TableDataCell(Action<ElementBuilder> b) => Tag("td", b);
        public static IHtmlContent Caption(Action<ElementBuilder> b) => Tag("caption", b);

        public static IHtmlContent CodeBlock(string language, Action<ElementBuilder> build)
            => Tag("pre", p =>
            {
                p.Tag("code", c =>
                {
                    c.Class($"language-{language}");
                    build(c);
                });
            });

        // Void tags
        public static IHtmlContent Br(Action<ElementBuilder> b) => VoidTag("br", b);
        public static IHtmlContent Hr(Action<ElementBuilder> b) => VoidTag("hr", b);
        public static IHtmlContent Img(Action<ElementBuilder> b) => VoidTag("img", b);

        public static IHtmlContent Input(Action<ElementBuilder> b) => VoidTag("input", b);

        public static IHtmlContent Input(Html.InputType type, Action<ElementBuilder> build)
            => VoidTag("input", b =>
            {
                b.Type(type);
                build(b);
            });

        public static IHtmlContent Input(Html.InputType type, params object?[] parts)
            => Html.Input(type, parts);

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
            // Attribute helpers (chainable)
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
            public ElementBuilder TitleAttr(string value) { _parts.Add(Html.TitleAttr(value)); return this; }
            public ElementBuilder Data(string key, string value) { _parts.Add(Html.Data(key, value)); return this; }
            public ElementBuilder Aria(string key, string value) { _parts.Add(Html.Aria(key, value)); return this; }

            public ElementBuilder Placeholder(string value) { _parts.Add(Html.Placeholder(value)); return this; }
            public ElementBuilder AutoComplete(string value) { _parts.Add(Html.AutoComplete(value)); return this; }
            public ElementBuilder Min(string value) { _parts.Add(Html.Min(value)); return this; }
            public ElementBuilder Max(string value) { _parts.Add(Html.Max(value)); return this; }
            public ElementBuilder Step(string value) { _parts.Add(Html.Step(value)); return this; }
            public ElementBuilder Pattern(string value) { _parts.Add(Html.Pattern(value)); return this; }
            public ElementBuilder MaxLength(int value) { _parts.Add(Html.MaxLength(value)); return this; }
            public ElementBuilder MinLength(int value) { _parts.Add(Html.MinLength(value)); return this; }
            public ElementBuilder Rows(int value) { _parts.Add(Html.Rows(value)); return this; }
            public ElementBuilder Cols(int value) { _parts.Add(Html.Cols(value)); return this; }
            public ElementBuilder Action(string value) { _parts.Add(Html.Action(value)); return this; }
            public ElementBuilder Method(string value) { _parts.Add(Html.Method(value)); return this; }
            public ElementBuilder EncType(string value) { _parts.Add(Html.EncType(value)); return this; }
            public ElementBuilder Rel(string value) { _parts.Add(Html.Rel(value)); return this; }
            public ElementBuilder Target(string value) { _parts.Add(Html.Target(value)); return this; }

            public ElementBuilder Disabled(bool on = true) { _parts.Add(Html.Disabled(on)); return this; }
            public ElementBuilder Checked(bool on = true) { _parts.Add(Html.Checked(on)); return this; }
            public ElementBuilder Selected(bool on = true) { _parts.Add(Html.Selected(on)); return this; }
            public ElementBuilder ReadOnly(bool on = true) { _parts.Add(Html.ReadOnly(on)); return this; }
            public ElementBuilder Required(bool on = true) { _parts.Add(Html.Required(on)); return this; }
            public ElementBuilder Multiple(bool on = true) { _parts.Add(Html.Multiple(on)); return this; }
            public ElementBuilder AutoFocus(bool on = true) { _parts.Add(Html.AutoFocus(on)); return this; }

            // ----------------------------
            // Nested tags (adds as children)
            // ----------------------------

            public ElementBuilder Tag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.Tag(name, build)); return this; }
            public ElementBuilder VoidTag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.VoidTag(name, build)); return this; }

            // Structure / head
            public ElementBuilder HtmlTag(Action<ElementBuilder> b) => Tag("html", b);
            public ElementBuilder Head(Action<ElementBuilder> b) => Tag("head", b);
            public ElementBuilder Body(Action<ElementBuilder> b) => Tag("body", b);
            public ElementBuilder Header(Action<ElementBuilder> b) => Tag("header", b);
            public ElementBuilder Main(Action<ElementBuilder> b) => Tag("main", b);
            public ElementBuilder Section(Action<ElementBuilder> b) => Tag("section", b);
            public ElementBuilder Article(Action<ElementBuilder> b) => Tag("article", b);
            public ElementBuilder Aside(Action<ElementBuilder> b) => Tag("aside", b);
            public ElementBuilder Footer(Action<ElementBuilder> b) => Tag("footer", b);
            public ElementBuilder Nav(Action<ElementBuilder> b) => Tag("nav", b);

            public ElementBuilder Title(Action<ElementBuilder> b) => Tag("title", b);
            public ElementBuilder Script(Action<ElementBuilder> b) => Tag("script", b);
            public ElementBuilder NoScript(Action<ElementBuilder> b) => Tag("noscript", b);
            public ElementBuilder Meta(Action<ElementBuilder> b) => VoidTag("meta", b);
            public ElementBuilder Link(Action<ElementBuilder> b) => VoidTag("link", b);

            // Content
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
            public ElementBuilder I(Action<ElementBuilder> b) => Tag("i", b);
            public ElementBuilder Code(Action<ElementBuilder> b) => Tag("code", b);
            public ElementBuilder Pre(Action<ElementBuilder> b) => Tag("pre", b);
            public ElementBuilder Template(Action<ElementBuilder> b) => Tag("template", b);

            // Figure
            public ElementBuilder Figure(Action<ElementBuilder> b) => Tag("figure", b);
            public ElementBuilder FigCaption(Action<ElementBuilder> b) => Tag("figcaption", b);

            // Forms
            public ElementBuilder Form(Action<ElementBuilder> b) => Tag("form", b);
            public ElementBuilder Label(Action<ElementBuilder> b) => Tag("label", b);
            public ElementBuilder TextArea(Action<ElementBuilder> b) => Tag("textarea", b);
            public ElementBuilder Textarea(Action<ElementBuilder> b) => Tag("textarea", b);
            public ElementBuilder Fieldset(Action<ElementBuilder> b) => Tag("fieldset", b);
            public ElementBuilder Legend(Action<ElementBuilder> b) => Tag("legend", b);

            // Selects
            public ElementBuilder Select(Action<ElementBuilder> b) => Tag("select", b);
            public ElementBuilder Option(Action<ElementBuilder> b) => Tag("option", b);
            public ElementBuilder OptGroup(Action<ElementBuilder> b) => Tag("optgroup", b);
            public ElementBuilder DataList(Action<ElementBuilder> b) => Tag("datalist", b);

            // Native disclosure / dialogs
            public ElementBuilder Details(Action<ElementBuilder> b) => Tag("details", b);
            public ElementBuilder Summary(Action<ElementBuilder> b) => Tag("summary", b);
            public ElementBuilder Dialog(Action<ElementBuilder> b) => Tag("dialog", b);

            // Tables
            public ElementBuilder TableHead(Action<ElementBuilder> b) => Tag("thead", b);
            public ElementBuilder TableBody(Action<ElementBuilder> b) => Tag("tbody", b);
            public ElementBuilder TableFoot(Action<ElementBuilder> b) => Tag("tfoot", b);
            public ElementBuilder TableRow(Action<ElementBuilder> b) => Tag("tr", b);
            public ElementBuilder TableHeaderCell(Action<ElementBuilder> b) => Tag("th", b);
            public ElementBuilder TableDataCell(Action<ElementBuilder> b) => Tag("td", b);
            public ElementBuilder Caption(Action<ElementBuilder> b) => Tag("caption", b);

            public ElementBuilder CodeBlock(string language, Action<ElementBuilder> build)
            {
                _parts.Add(FluentHtml.CodeBlock(language, build));
                return this;
            }

            // Void wrappers
            public ElementBuilder Br(Action<ElementBuilder> b) => VoidTag("br", b);
            public ElementBuilder Hr(Action<ElementBuilder> b) => VoidTag("hr", b);
            public ElementBuilder Img(Action<ElementBuilder> b) => VoidTag("img", b);
            public ElementBuilder Input(Action<ElementBuilder> b) => VoidTag("input", b);

            public ElementBuilder Input(Html.InputType type, Action<ElementBuilder> build)
            {
                _parts.Add(FluentHtml.Input(type, build));
                return this;
            }
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
            public FragmentBuilder Multiple(bool on = true) { _parts.Add(Html.Multiple(on)); return this; }
            public FragmentBuilder AutoFocus(bool on = true) { _parts.Add(Html.AutoFocus(on)); return this; }
            public FragmentBuilder Placeholder(string value) { _parts.Add(Html.Placeholder(value)); return this; }
            public FragmentBuilder AutoComplete(string value) { _parts.Add(Html.AutoComplete(value)); return this; }
            public FragmentBuilder Min(string value) { _parts.Add(Html.Min(value)); return this; }
            public FragmentBuilder Max(string value) { _parts.Add(Html.Max(value)); return this; }
            public FragmentBuilder Step(string value) { _parts.Add(Html.Step(value)); return this; }
            public FragmentBuilder Pattern(string value) { _parts.Add(Html.Pattern(value)); return this; }
            public FragmentBuilder MaxLength(int value) { _parts.Add(Html.MaxLength(value)); return this; }
            public FragmentBuilder MinLength(int value) { _parts.Add(Html.MinLength(value)); return this; }
            public FragmentBuilder Rows(int value) { _parts.Add(Html.Rows(value)); return this; }
            public FragmentBuilder Cols(int value) { _parts.Add(Html.Cols(value)); return this; }
            public FragmentBuilder Style(string css) { _parts.Add(Html.Style(css)); return this; }
            public FragmentBuilder Href(string href) { _parts.Add(Html.Href(href)); return this; }
            public FragmentBuilder Src(string src) { _parts.Add(Html.Src(src)); return this; }
            public FragmentBuilder Alt(string alt) { _parts.Add(Html.Alt(alt)); return this; }
            public FragmentBuilder Role(string role) { _parts.Add(Html.Role(role)); return this; }
            public FragmentBuilder ContentAttr(string value) { _parts.Add(Html.Content(value)); return this; }
            public FragmentBuilder TitleAttr(string value) { _parts.Add(Html.TitleAttr(value)); return this; }
            public FragmentBuilder Attr(string name, string? value) { _parts.Add(Html.Attr(name, value)); return this; }
            public FragmentBuilder Data(string key, string value) { _parts.Add(Html.Data(key, value)); return this; }
            public FragmentBuilder Aria(string key, string value) { _parts.Add(Html.Aria(key, value)); return this; }
            public FragmentBuilder Action(string value) { _parts.Add(Html.Action(value)); return this; }
            public FragmentBuilder Method(string value) { _parts.Add(Html.Method(value)); return this; }
            public FragmentBuilder EncType(string value) { _parts.Add(Html.EncType(value)); return this; }
            public FragmentBuilder Rel(string value) { _parts.Add(Html.Rel(value)); return this; }
            public FragmentBuilder Target(string value) { _parts.Add(Html.Target(value)); return this; }

            // Generic tag helpers
            public FragmentBuilder Tag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.Tag(name, build)); return this; }
            public FragmentBuilder VoidTag(string name, Action<ElementBuilder> build) { _parts.Add(FluentHtml.VoidTag(name, build)); return this; }

            // Structure
            public FragmentBuilder HtmlTag(Action<ElementBuilder> b) => Tag("html", b);
            public FragmentBuilder Head(Action<ElementBuilder> b) => Tag("head", b);
            public FragmentBuilder Body(Action<ElementBuilder> b) => Tag("body", b);
            public FragmentBuilder Header(Action<ElementBuilder> b) => Tag("header", b);
            public FragmentBuilder Main(Action<ElementBuilder> b) => Tag("main", b);
            public FragmentBuilder Section(Action<ElementBuilder> b) => Tag("section", b);
            public FragmentBuilder Article(Action<ElementBuilder> b) => Tag("article", b);
            public FragmentBuilder Aside(Action<ElementBuilder> b) => Tag("aside", b);
            public FragmentBuilder Footer(Action<ElementBuilder> b) => Tag("footer", b);
            public FragmentBuilder Nav(Action<ElementBuilder> b) => Tag("nav", b);

            public FragmentBuilder Title(Action<ElementBuilder> b) => Tag("title", b);
            public FragmentBuilder Script(Action<ElementBuilder> b) => Tag("script", b);
            public FragmentBuilder NoScript(Action<ElementBuilder> b) => Tag("noscript", b);
            public FragmentBuilder Meta(Action<ElementBuilder> b) => VoidTag("meta", b);
            public FragmentBuilder Link(Action<ElementBuilder> b) => VoidTag("link", b);

            // Content
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
            public FragmentBuilder I(Action<ElementBuilder> b) => Tag("i", b);
            public FragmentBuilder Code(Action<ElementBuilder> b) => Tag("code", b);
            public FragmentBuilder Pre(Action<ElementBuilder> b) => Tag("pre", b);
            public FragmentBuilder Template(Action<ElementBuilder> b) => Tag("template", b);

            // Figure
            public FragmentBuilder Figure(Action<ElementBuilder> b) => Tag("figure", b);
            public FragmentBuilder FigCaption(Action<ElementBuilder> b) => Tag("figcaption", b);

            // Forms
            public FragmentBuilder Form(Action<ElementBuilder> b) => Tag("form", b);
            public FragmentBuilder Label(Action<ElementBuilder> b) => Tag("label", b);
            public FragmentBuilder TextArea(Action<ElementBuilder> b) => Tag("textarea", b);
            public FragmentBuilder Textarea(Action<ElementBuilder> b) => Tag("textarea", b);
            public FragmentBuilder Fieldset(Action<ElementBuilder> b) => Tag("fieldset", b);
            public FragmentBuilder Legend(Action<ElementBuilder> b) => Tag("legend", b);

            // Selects
            public FragmentBuilder Select(Action<ElementBuilder> b) => Tag("select", b);
            public FragmentBuilder Option(Action<ElementBuilder> b) => Tag("option", b);
            public FragmentBuilder OptGroup(Action<ElementBuilder> b) => Tag("optgroup", b);
            public FragmentBuilder DataList(Action<ElementBuilder> b) => Tag("datalist", b);

            // Native disclosure / dialogs
            public FragmentBuilder Details(Action<ElementBuilder> b) => Tag("details", b);
            public FragmentBuilder Summary(Action<ElementBuilder> b) => Tag("summary", b);
            public FragmentBuilder Dialog(Action<ElementBuilder> b) => Tag("dialog", b);

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
        }

        // ============================================================
        // Internals
        // ============================================================

        private static IHtmlContent BuildTag(string name, bool isVoid, Action<ElementBuilder> build)
        {
            using var b = new ElementBuilder(initialCapacity: 12);
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
