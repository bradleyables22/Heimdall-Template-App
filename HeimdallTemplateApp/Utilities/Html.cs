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

        /// <summary>Creates id="...".</summary>
        public static HtmlAttr Id(string id) => Attr("id", id);

        /// <summary>Creates href="...".</summary>
        public static HtmlAttr Href(string href) => Attr("href", href);

        /// <summary>Creates src="...".</summary>
        public static HtmlAttr Src(string src) => Attr("src", src);

        /// <summary>Creates alt="...".</summary>
        public static HtmlAttr Alt(string alt) => Attr("alt", alt);

        /// <summary>Creates type="...".</summary>
        public static HtmlAttr Type(string type) => Attr("type", type);

        /// <summary>Creates type="..." from an <see cref="InputType"/> enum.</summary>
        public static HtmlAttr Type(InputType type) => Attr("type", ToInputTypeString(type));

        /// <summary>Creates name="...".</summary>
        public static HtmlAttr Name(string name) => Attr("name", name);

        /// <summary>Creates value="...".</summary>
        public static HtmlAttr Value(string value) => Attr("value", value);

        /// <summary>Creates role="...".</summary>
        public static HtmlAttr Role(string role) => Attr("role", role);

        /// <summary>Creates style="...".</summary>
        public static HtmlAttr Style(string css) => Attr("style", css);

        /// <summary>Creates content="..." (commonly used in meta tags).</summary>
        public static HtmlAttr Content(string value) => Attr("content", value);

        /// <summary>
        /// Creates for="..." (used on &lt;label&gt; to target an element id).
        /// In HTML, "for" should match the target element's id.
        /// </summary>
        public static HtmlAttr For(string value) => Attr("for", value);

        /// <summary>Creates data-{key}="value".</summary>
        public static HtmlAttr Data(string key, string value)
            => Attr($"data-{key}", value);

        /// <summary>Creates aria-{key}="value".</summary>
        public static HtmlAttr Aria(string key, string value)
            => Attr($"aria-{key}", value);

        /// <summary>Creates the boolean attribute disabled.</summary>
        public static HtmlAttr Disabled(bool on = true) => Bool("disabled", on);

        /// <summary>Creates the boolean attribute checked.</summary>
        public static HtmlAttr Checked(bool on = true) => Bool("checked", on);

        /// <summary>Creates the boolean attribute selected.</summary>
        public static HtmlAttr Selected(bool on = true) => Bool("selected", on);

        /// <summary>Creates the boolean attribute readonly.</summary>
        public static HtmlAttr ReadOnly(bool on = true) => Bool("readonly", on);

        /// <summary>Creates the boolean attribute required.</summary>
        public static HtmlAttr Required(bool on = true) => Bool("required", on);

        // ============================================================
        // Common tags (ergonomic wrappers)
        // ============================================================

        /// <summary>Creates &lt;title&gt;...&lt;/title&gt;.</summary>
        public static IHtmlContent Title(params object?[] c) => Tag("title", c);

        /// <summary>Creates &lt;div&gt;...&lt;/div&gt;.</summary>
        public static IHtmlContent Div(params object?[] c) => Tag("div", c);

        /// <summary>Creates &lt;span&gt;...&lt;/span&gt;.</summary>
        public static IHtmlContent Span(params object?[] c) => Tag("span", c);

        /// <summary>Creates &lt;strong&gt;...&lt;/strong&gt;.</summary>
        public static IHtmlContent Strong(params object?[] c) => Tag("strong", c);

        /// <summary>Creates &lt;p&gt;...&lt;/p&gt;.</summary>
        public static IHtmlContent P(params object?[] c) => Tag("p", c);

        /// <summary>Creates &lt;h1&gt;...&lt;/h1&gt;.</summary>
        public static IHtmlContent H1(params object?[] c) => Tag("h1", c);

        /// <summary>Creates &lt;h2&gt;...&lt;/h2&gt;.</summary>
        public static IHtmlContent H2(params object?[] c) => Tag("h2", c);

        /// <summary>Creates &lt;h3&gt;...&lt;/h3&gt;.</summary>
        public static IHtmlContent H3(params object?[] c) => Tag("h3", c);

        /// <summary>Creates &lt;h4&gt;...&lt;/h4&gt;.</summary>
        public static IHtmlContent H4(params object?[] c) => Tag("h4", c);

        /// <summary>Creates &lt;h5&gt;...&lt;/h5&gt;.</summary>
        public static IHtmlContent H5(params object?[] c) => Tag("h5", c);

        /// <summary>Creates &lt;h6&gt;...&lt;/h6&gt;.</summary>
        public static IHtmlContent H6(params object?[] c) => Tag("h6", c);
        // <summary>Creates &lt;textarea&gt;...&lt;/textarea&gt;.</summary>
        public static IHtmlContent TextArea(params object?[] c) => Tag("textarea", c);
        /// <summary>Creates &lt;ul&gt;...&lt;/ul&gt;.</summary>
        public static IHtmlContent Ul(params object?[] c) => Tag("ul", c);

        /// <summary>Creates &lt;li&gt;...&lt;/li&gt;.</summary>
        public static IHtmlContent Li(params object?[] c) => Tag("li", c);

        /// <summary>Creates &lt;a&gt;...&lt;/a&gt;.</summary>
        public static IHtmlContent A(params object?[] c) => Tag("a", c);

        /// <summary>Creates &lt;button&gt;...&lt;/button&gt;.</summary>
        public static IHtmlContent Button(params object?[] c) => Tag("button", c);

        /// <summary>Creates &lt;nav&gt;...&lt;/nav&gt;.</summary>
        public static IHtmlContent Nav(params object?[] c) => Tag("nav", c);

        /// <summary>Creates &lt;form&gt;...&lt;/form&gt;.</summary>
        public static IHtmlContent Form(params object?[] c) => Tag("form", c);

        /// <summary>Creates &lt;label&gt;...&lt;/label&gt;.</summary>
        public static IHtmlContent Label(params object?[] c) => Tag("label", c);

        /// <summary>Creates &lt;i&gt;...&lt;/i&gt;.</summary>
        public static IHtmlContent I(params object?[] c) => Tag("i", c);

        /// <summary>Creates &lt;code&gt;...&lt;/code&gt;.</summary>
        public static IHtmlContent Code(params object?[] c) => Tag("code", c);

        /// <summary>Creates &lt;pre&gt;...&lt;/pre&gt;.</summary>
        public static IHtmlContent Pre(params object?[] c) => Tag("pre", c);

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

        /// <summary>Creates &lt;thead&gt;...&lt;/thead&gt;.</summary>
        public static IHtmlContent TableHead(params object?[] c) => Tag("thead", c);

        /// <summary>Creates &lt;tbody&gt;...&lt;/tbody&gt;.</summary>
        public static IHtmlContent TableBody(params object?[] c) => Tag("tbody", c);

        /// <summary>Creates &lt;tfoot&gt;...&lt;/tfoot&gt;.</summary>
        public static IHtmlContent TableFoot(params object?[] c) => Tag("tfoot", c);

        /// <summary>Creates &lt;tr&gt;...&lt;/tr&gt;.</summary>
        public static IHtmlContent TableRow(params object?[] c) => Tag("tr", c);

        /// <summary>Creates &lt;th&gt;...&lt;/th&gt;.</summary>
        public static IHtmlContent TableHeaderCell(params object?[] c) => Tag("th", c);

        /// <summary>Creates &lt;td&gt;...&lt;/td&gt;.</summary>
        public static IHtmlContent TableDataCell(params object?[] c) => Tag("td", c);

        /// <summary>Creates &lt;caption&gt;...&lt;/caption&gt;.</summary>
        public static IHtmlContent Caption(params object?[] c) => Tag("caption", c);

        /// <summary>Creates &lt;script&gt;...&lt;/script&gt;.</summary>
        public static IHtmlContent Script(params object?[] c) => Tag("script", c);

        /// <summary>Creates &lt;noscript&gt;...&lt;/noscript&gt;.</summary>
        public static IHtmlContent NoScript(params object?[] c) => Tag("noscript", c);

        /// <summary>Creates &lt;template&gt;...&lt;/template&gt;.</summary>
        public static IHtmlContent Template(params object?[] c) => Tag("template", c);

        /// <summary>Creates &lt;br /&gt;.</summary>
        public static IHtmlContent Br(params object?[] c) => VoidTag("br", c);

        /// <summary>Creates &lt;hr /&gt;.</summary>
        public static IHtmlContent Hr(params object?[] c) => VoidTag("hr", c);

        /// <summary>Creates &lt;img /&gt;.</summary>
        public static IHtmlContent Img(params object?[] c) => VoidTag("img", c);

        /// <summary>Creates &lt;input /&gt;. Add attributes like <see cref="Type(string)"/> / <see cref="Type(InputType)"/>.</summary>
        public static IHtmlContent Input(params object?[] c) => VoidTag("input", c);

        /// <summary>
        /// Creates &lt;input type="{type}" /&gt;.
        /// This overload is the correct way to use the <see cref="InputType"/> enum.
        /// </summary>
        public static IHtmlContent Input(InputType type, params object?[] c)
            => VoidTag("input", Type(type), c);

        /// <summary>Creates &lt;meta /&gt;.</summary>
        public static IHtmlContent Meta(params object?[] c) => VoidTag("meta", c);

        /// <summary>Creates &lt;link /&gt;.</summary>
        public static IHtmlContent Link(params object?[] c) => VoidTag("link", c);

        /// <summary>Creates &lt;html&gt;...&lt;/html&gt;.</summary>
        public static IHtmlContent HtmlTag(params object?[] c) => Tag("html", c);

        /// <summary>Creates &lt;head&gt;...&lt;/head&gt;.</summary>
        public static IHtmlContent Head(params object?[] c) => Tag("head", c);

        /// <summary>Creates &lt;body&gt;...&lt;/body&gt;.</summary>
        public static IHtmlContent Body(params object?[] c) => Tag("body", c);

        /// <summary>Creates &lt;footer&gt;...&lt;/footer&gt;.</summary>
        public static IHtmlContent Footer(params object?[] c) => Tag("footer", c);

        // ============================================================
        // Internals
        // ============================================================

        /// <summary>
        /// Joins CSS class strings with a single space, trimming each segment and skipping empty values.
        /// </summary>
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

        /// <summary>
        /// Classifies attribute behavior for writing/merging.
        /// </summary>
        public enum AttrKind { None, Normal, Boolean, Class }

        /// <summary>
        /// Represents a single HTML attribute.
        /// - Normal/Class: name="value" (value is encoded)
        /// - Boolean: name (presence-only; no ="" emitted)
        /// </summary>
        public readonly record struct HtmlAttr(string Name, string Value, AttrKind Kind)
        {
            /// <summary>An empty/no-op attribute.</summary>
            public static readonly HtmlAttr Empty = new("", "", AttrKind.None);

            /// <summary>True when this attribute represents no output.</summary>
            public bool IsEmpty => Kind == AttrKind.None || string.IsNullOrWhiteSpace(Name);

            /// <summary>
            /// Writes this attribute to the provided writer, encoding values as needed.
            /// </summary>
            internal void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                if (IsEmpty)
                    return;

                writer.Write(' ');
                writer.Write(Name);

                // Boolean attributes are represented by presence:
                //   <input disabled>
                if (Kind == AttrKind.Boolean)
                    return;

                writer.Write("=\"");
                encoder.Encode(writer, Value);
                writer.Write('"');
            }
        }

        /// <summary>
        /// Flattens nested arrays/enumerables produced by helpers returning collections of parts.
        /// IMPORTANT:
        /// - Treat <see cref="IHtmlContent"/> as atomic (do NOT enumerate it).
        /// - Treat string as atomic (string is IEnumerable&lt;char&gt;).
        /// </summary>
        private static IEnumerable<object?> Flatten(IEnumerable parts)
        {
            foreach (var p in parts)
            {
                if (p is null)
                    continue;

                // Do NOT flatten/iterate these.
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

        /// <summary>
        /// Small ArrayPool-backed buffer builder: Rent -> Add -> ToArray -> Return.
        /// This reduces per-element allocations when building larger node trees.
        /// </summary>
        private struct PooledBuffer<T>
        {
            private T[]? _arr;
            private int _count;

            /// <summary>The number of items currently stored.</summary>
            public int Count => _count;

            /// <summary>
            /// The underlying buffer (only valid after <see cref="Init(int)"/>).
            /// Used internally for in-place overwrite/merge operations.
            /// </summary>
            internal T[] Buffer
                => _arr ?? throw new InvalidOperationException("Buffer not initialized.");

            /// <summary>
            /// Initializes the buffer by renting from the shared ArrayPool.
            /// </summary>
            public void Init(int initialCapacity = 8)
            {
                _arr = ArrayPool<T>.Shared.Rent(initialCapacity);
                _count = 0;
            }

            /// <summary>
            /// Adds an item, growing the rented buffer if needed.
            /// </summary>
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

            /// <summary>
            /// Copies the used portion of the rented buffer into a new exact-length array.
            /// </summary>
            public T[] ToArray()
            {
                if (_arr is null || _count == 0)
                    return Array.Empty<T>();

                var result = new T[_count];
                Array.Copy(_arr, 0, result, 0, _count);
                return result;
            }

            /// <summary>
            /// Returns the rented buffer to the ArrayPool.
            /// </summary>
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

        /// <summary>
        /// A fragment node that renders a sequence of parts without a wrapping element.
        /// </summary>
        private sealed class FragmentNode : IHtmlContent
        {
            private readonly object?[] _parts;
            private FragmentNode(object?[] parts) => _parts = parts;

            /// <summary>
            /// Creates a fragment node. Null is treated as an empty fragment.
            /// </summary>
            public static IHtmlContent Create(object?[] parts)
                => new FragmentNode(parts ?? Array.Empty<object?>());

            /// <summary>
            /// Writes all parts in order.
            /// </summary>
            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                foreach (var p in _parts)
                    RenderPart(writer, encoder, p);
            }
        }

        /// <summary>
        /// A node representing plain text that will be encoded on output.
        /// </summary>
        private sealed class TextNode : IHtmlContent
        {
            private readonly string _text;

            /// <summary>Creates a text node.</summary>
            public TextNode(string text) => _text = text;

            /// <summary>Writes the encoded text.</summary>
            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
                => encoder.Encode(writer, _text);
        }

        /// <summary>
        /// A node representing a single HTML element with attributes and children.
        /// </summary>
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

            /// <summary>
            /// Splits parts into attributes and children, applying merge rules.
            ///
            /// Rules:
            /// - Stable ordering: first-seen attribute position is preserved.
            /// - Overwrite-by-name (case-insensitive) for normal/boolean attrs.
            /// - Class attributes are merged into the first-seen class attribute.
            /// - Boolean attributes render as presence-only (e.g. disabled).
            /// - Nested enumerables are flattened (see <see cref="Flatten(IEnumerable)"/>).
            /// - Uses pooled buffers to reduce allocations.
            /// </summary>
            public static IHtmlContent Create(string name, bool isVoid, object?[] parts)
            {
                var attrs = new PooledBuffer<HtmlAttr>();
                var children = new PooledBuffer<object?>();

                attrs.Init(initialCapacity: 6);
                children.Init(initialCapacity: 6);

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
                                    // Merge onto the first-seen class attribute.
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

                            // Overwrite existing by name (case-insensitive), preserving attribute order.
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

            /// <summary>
            /// Writes this element as HTML.
            /// Void elements are written using "/&gt;" style (e.g. &lt;input /&gt;).
            /// </summary>
            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                writer.Write('<');
                writer.Write(_name);

                // Stable attribute order.
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

        /// <summary>
        /// Renders a single "part" (child) to the writer.
        /// Strings and arbitrary objects are encoded; <see cref="IHtmlContent"/> is trusted to render itself.
        /// Enumerables are iterated (except strings and IHtmlContent which are treated as atomic earlier).
        /// </summary>
        private static void RenderPart(TextWriter writer, HtmlEncoder encoder, object? part)
        {
            if (part is null)
                return;

            switch (part)
            {
                case HtmlAttr:
                    // Attributes are consumed during ElementNode.Create; if they show up in children, ignore.
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
