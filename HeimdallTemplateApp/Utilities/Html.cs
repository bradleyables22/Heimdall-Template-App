using Microsoft.AspNetCore.Html;
using System.Buffers;
using System.Collections;
using System.Text;
using System.Text.Encodings.Web;

namespace HeimdallTemplateApp.Utilities
{
    /// <summary>
    /// Lightweight HTML rendering core.
    ///
    /// Design goals:
    /// - Small, predictable API for constructing HTML nodes (Tag/VoidTag/Fragment/Text/Raw).
    /// - Safe output by default (strings are HTML-encoded; use <see cref="Raw(string?)"/> intentionally).
    /// - Attribute merging rules (stable order, overwrite-by-name, class merging, boolean attributes).
    /// - Low-GC implementation (ArrayPool-backed buffers during node construction).
    /// - Supports nested enumerables via flattening, enabling helpers that return arrays/enumerables of parts.
    /// </summary>
    public static class Html
    {
        /// <summary>
        /// Strongly-typed HTML input types.
        /// Values are converted to the correct HTML string (underscores become hyphens).
        /// Example: <see cref="datetime_local"/> becomes "datetime-local".
        /// </summary>
        public enum InputType
        {
            button,
            checkbox,
            color,
            date,
            datetime_local,
            email,
            file,
            hidden,
            image,
            month,
            number,
            password,
            radio,
            range,
            reset,
            search,
            submit,
            tel,
            text,
            time,
            url,
            week
        }

        // ============================================================
        // Element factories
        // ============================================================

        /// <summary>
        /// Creates a non-void element node (e.g. &lt;div&gt;...&lt;/div&gt;).
        /// Parts may include attributes (<see cref="HtmlAttr"/>) and/or children (strings, <see cref="IHtmlContent"/>, objects).
        /// </summary>
        public static IHtmlContent Tag(string name, params object?[] parts)
            => ElementNode.Create(name, isVoid: false, parts);

        /// <summary>
        /// Creates a void element node (e.g. &lt;input /&gt;).
        /// Parts may include attributes (<see cref="HtmlAttr"/>) and/or children (children are ignored for void tags).
        /// </summary>
        public static IHtmlContent VoidTag(string name, params object?[] parts)
            => ElementNode.Create(name, isVoid: true, parts);

        /// <summary>
        /// Creates a fragment node that simply renders each part in sequence.
        /// Useful for conditional or repeated content without introducing wrapper elements.
        /// </summary>
        public static IHtmlContent Fragment(params object?[] parts)
            => FragmentNode.Create(parts);

        /// <summary>
        /// Creates a text node that will be HTML-encoded when rendered.
        /// </summary>
        public static IHtmlContent Text(string? text)
            => new TextNode(text ?? string.Empty);

        /// <summary>
        /// Creates a raw HTML node. The provided HTML will be written as-is (no encoding).
        /// Use sparingly and only with trusted content.
        /// </summary>
        public static IHtmlContent Raw(string? html)
            => new HtmlString(html ?? string.Empty);

        // ============================================================
        // Attribute helpers
        // ============================================================

        /// <summary>
        /// Creates a normal attribute: name="value" (value is HTML-encoded).
        /// Returns <see cref="HtmlAttr.Empty"/> if name is empty/whitespace.
        /// </summary>
        public static HtmlAttr Attr(string name, string? value)
            => string.IsNullOrWhiteSpace(name)
                ? HtmlAttr.Empty
                : new HtmlAttr(name, value ?? string.Empty, AttrKind.Normal);

        /// <summary>
        /// Creates a boolean attribute (presence-only). When <paramref name="on"/> is true:
        /// emits " name" (no ="").
        /// Returns <see cref="HtmlAttr.Empty"/> if off or name is empty.
        /// </summary>
        public static HtmlAttr Bool(string name, bool on)
            => on && !string.IsNullOrWhiteSpace(name)
                ? new HtmlAttr(name, name, AttrKind.Boolean)
                : HtmlAttr.Empty;

        /// <summary>
        /// Creates a class attribute. Class values are joined with a single space and trimmed.
        /// Multiple class attributes on the same element are merged.
        /// </summary>
        public static HtmlAttr Class(params string?[] classes)
            => new HtmlAttr("class", CssJoin(classes), AttrKind.Class);

        /// <summary>
        /// Converts <see cref="InputType"/> enum values into HTML string values.
        /// Example: datetime_local -> "datetime-local".
        /// </summary>
        private static string ToInputTypeString(InputType type)
            => type.ToString().Replace("_", "-");

        // ---- common attributes ----

        public static HtmlAttr Id(string id) => Attr("id", id);
        public static HtmlAttr Href(string href) => Attr("href", href);
        public static HtmlAttr Src(string src) => Attr("src", src);
        public static HtmlAttr Alt(string alt) => Attr("alt", alt);
        public static HtmlAttr Type(string type) => Attr("type", type);
        public static HtmlAttr Type(InputType type) => Attr("type", ToInputTypeString(type));
        public static HtmlAttr Name(string name) => Attr("name", name);
        public static HtmlAttr Value(string value) => Attr("value", value);
        public static HtmlAttr Role(string role) => Attr("role", role);
        public static HtmlAttr Style(string css) => Attr("style", css);
        public static HtmlAttr Content(string value) => Attr("content", value);
        public static HtmlAttr For(string value) => Attr("for", value);

        /// <summary>Creates title="..." (tooltip/accessibility label on many tags).</summary>
        public static HtmlAttr TitleAttr(string value) => Attr("title", value);

        public static HtmlAttr Data(string key, string value) => Attr($"data-{key}", value);
        public static HtmlAttr Aria(string key, string value) => Attr($"aria-{key}", value);

        // ---- form/text helpers (high-value ergonomics) ----

        public static HtmlAttr Placeholder(string value) => Attr("placeholder", value);
        public static HtmlAttr AutoComplete(string value) => Attr("autocomplete", value);
        public static HtmlAttr Min(string value) => Attr("min", value);
        public static HtmlAttr Max(string value) => Attr("max", value);
        public static HtmlAttr Step(string value) => Attr("step", value);
        public static HtmlAttr Pattern(string value) => Attr("pattern", value);
        public static HtmlAttr MaxLength(int value) => Attr("maxlength", value.ToString());
        public static HtmlAttr MinLength(int value) => Attr("minlength", value.ToString());
        public static HtmlAttr Rows(int value) => Attr("rows", value.ToString());
        public static HtmlAttr Cols(int value) => Attr("cols", value.ToString());

        public static HtmlAttr Action(string value) => Attr("action", value);
        public static HtmlAttr Method(string value) => Attr("method", value);
        public static HtmlAttr EncType(string value) => Attr("enctype", value);

        public static HtmlAttr Rel(string value) => Attr("rel", value);
        public static HtmlAttr Target(string value) => Attr("target", value);

        // ---- boolean attributes ----

        public static HtmlAttr Disabled(bool on = true) => Bool("disabled", on);
        public static HtmlAttr Checked(bool on = true) => Bool("checked", on);
        public static HtmlAttr Selected(bool on = true) => Bool("selected", on);
        public static HtmlAttr ReadOnly(bool on = true) => Bool("readonly", on);
        public static HtmlAttr Required(bool on = true) => Bool("required", on);
        public static HtmlAttr Multiple(bool on = true) => Bool("multiple", on);
        public static HtmlAttr AutoFocus(bool on = true) => Bool("autofocus", on);

        // ============================================================
        // Common tags (ergonomic wrappers)
        // ============================================================

        // Document structure
        public static IHtmlContent HtmlTag(params object?[] c) => Tag("html", c);
        public static IHtmlContent Head(params object?[] c) => Tag("head", c);
        public static IHtmlContent Body(params object?[] c) => Tag("body", c);
        public static IHtmlContent Header(params object?[] c) => Tag("header", c);
        public static IHtmlContent Main(params object?[] c) => Tag("main", c);
        public static IHtmlContent Section(params object?[] c) => Tag("section", c);
        public static IHtmlContent Article(params object?[] c) => Tag("article", c);
        public static IHtmlContent Aside(params object?[] c) => Tag("aside", c);
        public static IHtmlContent Footer(params object?[] c) => Tag("footer", c);
        public static IHtmlContent Nav(params object?[] c) => Tag("nav", c);

        // Head tags
        public static IHtmlContent Title(params object?[] c) => Tag("title", c);
        public static IHtmlContent Meta(params object?[] c) => VoidTag("meta", c);
        public static IHtmlContent Link(params object?[] c) => VoidTag("link", c);
        public static IHtmlContent Script(params object?[] c) => Tag("script", c);
        public static IHtmlContent NoScript(params object?[] c) => Tag("noscript", c);

        // Text/content
        public static IHtmlContent Div(params object?[] c) => Tag("div", c);
        public static IHtmlContent Span(params object?[] c) => Tag("span", c);
        public static IHtmlContent Strong(params object?[] c) => Tag("strong", c);
        public static IHtmlContent P(params object?[] c) => Tag("p", c);
        public static IHtmlContent H1(params object?[] c) => Tag("h1", c);
        public static IHtmlContent H2(params object?[] c) => Tag("h2", c);
        public static IHtmlContent H3(params object?[] c) => Tag("h3", c);
        public static IHtmlContent H4(params object?[] c) => Tag("h4", c);
        public static IHtmlContent H5(params object?[] c) => Tag("h5", c);
        public static IHtmlContent H6(params object?[] c) => Tag("h6", c);
        public static IHtmlContent A(params object?[] c) => Tag("a", c);
        public static IHtmlContent Button(params object?[] c) => Tag("button", c);
        public static IHtmlContent Code(params object?[] c) => Tag("code", c);
        public static IHtmlContent Pre(params object?[] c) => Tag("pre", c);
        public static IHtmlContent Template(params object?[] c) => Tag("template", c);

        // Lists
        public static IHtmlContent Ul(params object?[] c) => Tag("ul", c);
        public static IHtmlContent Li(params object?[] c) => Tag("li", c);

        // Figure
        public static IHtmlContent Figure(params object?[] c) => Tag("figure", c);
        public static IHtmlContent FigCaption(params object?[] c) => Tag("figcaption", c);

        // Forms
        public static IHtmlContent Form(params object?[] c) => Tag("form", c);
        public static IHtmlContent Label(params object?[] c) => Tag("label", c);
        public static IHtmlContent TextArea(params object?[] c) => Tag("textarea", c);
        public static IHtmlContent Fieldset(params object?[] c) => Tag("fieldset", c);
        public static IHtmlContent Legend(params object?[] c) => Tag("legend", c);

        // Selects
        public static IHtmlContent Select(params object?[] c) => Tag("select", c);
        public static IHtmlContent Option(params object?[] c) => Tag("option", c);
        public static IHtmlContent OptGroup(params object?[] c) => Tag("optgroup", c);
        public static IHtmlContent DataList(params object?[] c) => Tag("datalist", c);

        // Native disclosure / dialogs
        public static IHtmlContent Details(params object?[] c) => Tag("details", c);
        public static IHtmlContent Summary(params object?[] c) => Tag("summary", c);
        public static IHtmlContent Dialog(params object?[] c) => Tag("dialog", c);

        // Tables
        public static IHtmlContent TableHead(params object?[] c) => Tag("thead", c);
        public static IHtmlContent TableBody(params object?[] c) => Tag("tbody", c);
        public static IHtmlContent TableFoot(params object?[] c) => Tag("tfoot", c);
        public static IHtmlContent TableRow(params object?[] c) => Tag("tr", c);
        public static IHtmlContent TableHeaderCell(params object?[] c) => Tag("th", c);
        public static IHtmlContent TableDataCell(params object?[] c) => Tag("td", c);
        public static IHtmlContent Caption(params object?[] c) => Tag("caption", c);

        // Void tags
        public static IHtmlContent Br(params object?[] c) => VoidTag("br", c);
        public static IHtmlContent Hr(params object?[] c) => VoidTag("hr", c);
        public static IHtmlContent Img(params object?[] c) => VoidTag("img", c);

        public static IHtmlContent Input(params object?[] c) => VoidTag("input", c);

        public static IHtmlContent Input(InputType type, params object?[] c)
            => VoidTag("input", Type(type), c);

        /// <summary>
        /// Creates a code block: &lt;pre&gt;&lt;code class="language-{language}"&gt;...&lt;/code&gt;&lt;/pre&gt;.
        /// </summary>
        public static IHtmlContent CodeBlock(string language, params object?[] c)
            => Tag("pre",
                Tag("code",
                    Class($"language-{language}"),
                    c
                )
            );

        // ============================================================
        // Internals
        // ============================================================

        private static string CssJoin(IEnumerable<string?> classes)
        {
            var sb = new StringBuilder();
            foreach (var c in classes)
            {
                if (string.IsNullOrWhiteSpace(c))
                    continue;

                if (sb.Length > 0)
                    sb.Append(' ');

                sb.Append(c.Trim());
            }
            return sb.ToString();
        }

        public enum AttrKind { None, Normal, Boolean, Class }

        public readonly record struct HtmlAttr(string Name, string Value, AttrKind Kind)
        {
            public static readonly HtmlAttr Empty = new("", "", AttrKind.None);

            public bool IsEmpty => Kind == AttrKind.None || string.IsNullOrWhiteSpace(Name);

            internal void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                if (IsEmpty)
                    return;

                writer.Write(' ');
                writer.Write(Name);

                if (Kind == AttrKind.Boolean)
                    return;

                writer.Write("=\"");
                encoder.Encode(writer, Value);
                writer.Write('"');
            }
        }

        private static IEnumerable<object?> Flatten(IEnumerable parts)
        {
            foreach (var p in parts)
            {
                if (p is null)
                    continue;

                if (p is IHtmlContent || p is string)
                {
                    yield return p;
                    continue;
                }

                if (p is IEnumerable seq)
                {
                    foreach (var x in Flatten(seq))
                        yield return x;
                    continue;
                }

                yield return p;
            }
        }

        private struct PooledBuffer<T>
        {
            private T[]? _arr;
            private int _count;

            public int Count => _count;

            internal T[] Buffer
                => _arr ?? throw new InvalidOperationException("Buffer not initialized.");

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

        private sealed class FragmentNode : IHtmlContent
        {
            private readonly object?[] _parts;
            private FragmentNode(object?[] parts) => _parts = parts;

            public static IHtmlContent Create(object?[] parts)
                => new FragmentNode(parts ?? Array.Empty<object?>());

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                foreach (var p in _parts)
                    RenderPart(writer, encoder, p);
            }
        }

        private sealed class TextNode : IHtmlContent
        {
            private readonly string _text;
            public TextNode(string text) => _text = text;

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
                => encoder.Encode(writer, _text);
        }

        private sealed class ElementNode : IHtmlContent
        {
            private readonly string _name;
            private readonly bool _isVoid;
            private readonly HtmlAttr[] _attrs;
            private readonly object?[] _children;

            private ElementNode(string name, bool isVoid, HtmlAttr[] attrs, object?[] children)
            {
                _name = name;
                _isVoid = isVoid;
                _attrs = attrs;
                _children = children;
            }

            public static IHtmlContent Create(string name, bool isVoid, object?[] parts)
            {
                var attrs = new PooledBuffer<HtmlAttr>();
                var children = new PooledBuffer<object?>();

                attrs.Init(initialCapacity: 8);
                children.Init(initialCapacity: 8);

                int classIndex = -1;

                try
                {
                    foreach (var part in Flatten(parts ?? Array.Empty<object?>()))
                    {
                        if (part is HtmlAttr attr)
                        {
                            if (attr.IsEmpty)
                                continue;

                            if (attr.Kind == AttrKind.Class)
                            {
                                if (string.IsNullOrWhiteSpace(attr.Value))
                                    continue;

                                if (classIndex < 0)
                                {
                                    classIndex = attrs.Count;
                                    attrs.Add(attr);
                                }
                                else
                                {
                                    var existing = attrs.Buffer[classIndex];
                                    attrs.Buffer[classIndex] = new HtmlAttr(
                                        "class",
                                        string.IsNullOrWhiteSpace(existing.Value)
                                            ? attr.Value
                                            : $"{existing.Value} {attr.Value}",
                                        AttrKind.Class);
                                }

                                continue;
                            }

                            var replaced = false;
                            for (int i = 0; i < attrs.Count; i++)
                            {
                                var existing = attrs.Buffer[i];
                                if (existing.IsEmpty)
                                    continue;

                                if (string.Equals(existing.Name, attr.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    attrs.Buffer[i] = attr;
                                    replaced = true;
                                    break;
                                }
                            }

                            if (!replaced)
                                attrs.Add(attr);
                        }
                        else
                        {
                            children.Add(part);
                        }
                    }

                    return new ElementNode(
                        name,
                        isVoid,
                        attrs.ToArray(),
                        children.ToArray());
                }
                finally
                {
                    attrs.Dispose();
                    children.Dispose();
                }
            }

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                writer.Write('<');
                writer.Write(_name);

                foreach (var attr in _attrs)
                    attr.WriteTo(writer, encoder);

                if (_isVoid)
                {
                    writer.Write(" />");
                    return;
                }

                writer.Write('>');

                foreach (var child in _children)
                    RenderPart(writer, encoder, child);

                writer.Write("</");
                writer.Write(_name);
                writer.Write('>');
            }
        }

        private static void RenderPart(TextWriter writer, HtmlEncoder encoder, object? part)
        {
            if (part is null)
                return;

            switch (part)
            {
                case HtmlAttr:
                    return;

                case IHtmlContent html:
                    html.WriteTo(writer, encoder);
                    return;

                case string s:
                    encoder.Encode(writer, s);
                    return;

                case IEnumerable seq:
                    foreach (var item in seq)
                        RenderPart(writer, encoder, item);
                    return;

                default:
                    encoder.Encode(writer, part.ToString() ?? "");
                    return;
            }
        }
    }
}
