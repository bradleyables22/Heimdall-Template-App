using Microsoft.AspNetCore.Html;
using System.Buffers;
using System.Collections;
using System.Text;
using System.Text.Encodings.Web;

namespace Server.Utilities
{
    public static partial class Html
    {
        // ============================================================
        // Element factories
        // ============================================================

        public static IHtmlContent Tag(string name, params object?[] parts)
            => ElementNode.Create(name, isVoid: false, parts);

        public static IHtmlContent VoidTag(string name, params object?[] parts)
            => ElementNode.Create(name, isVoid: true, parts);

        public static IHtmlContent Fragment(params object?[] parts)
            => FragmentNode.Create(parts);

        public static IHtmlContent Text(string? text)
            => new TextNode(text ?? string.Empty);

        public static IHtmlContent Raw(string? html)
            => new HtmlString(html ?? string.Empty);


        // ============================================================
        // Attribute helpers
        // ============================================================

        public static HtmlAttr Attr(string name, string? value)
            => string.IsNullOrWhiteSpace(name)
                ? HtmlAttr.Empty
                : new HtmlAttr(name, value ?? string.Empty, AttrKind.Normal);

        public static HtmlAttr Bool(string name, bool on)
            => on && !string.IsNullOrWhiteSpace(name)
                ? new HtmlAttr(name, name, AttrKind.Boolean)
                : HtmlAttr.Empty;

        public static HtmlAttr Class(params string?[] classes)
            => new HtmlAttr("class", CssJoin(classes), AttrKind.Class);

        public static HtmlAttr Id(string id) => Attr("id", id);
        public static HtmlAttr Href(string href) => Attr("href", href);
        public static HtmlAttr Src(string src) => Attr("src", src);
        public static HtmlAttr Alt(string alt) => Attr("alt", alt);
        public static HtmlAttr Type(string type) => Attr("type", type);
        public static HtmlAttr Name(string name) => Attr("name", name);
        public static HtmlAttr Value(string value) => Attr("value", value);
        public static HtmlAttr Role(string role) => Attr("role", role);
        public static HtmlAttr Style(string css) => Attr("style", css);
        public static HtmlAttr Content(string value) => Attr("content", value);
        public static HtmlAttr Data(string key, string value)
            => Attr($"data-{key}", value);

        public static HtmlAttr Aria(string key, string value)
            => Attr($"aria-{key}", value);

        public static HtmlAttr Disabled(bool on = true) => Bool("disabled", on);
        public static HtmlAttr Checked(bool on = true) => Bool("checked", on);
        public static HtmlAttr Selected(bool on = true) => Bool("selected", on);
        public static HtmlAttr ReadOnly(bool on = true) => Bool("readonly", on);
        public static HtmlAttr Required(bool on = true) => Bool("required", on);

        // ============================================================
        // Common tags (ergonomic wrappers)
        // ============================================================

        public static IHtmlContent Title(params object?[] c) => Tag("title", c);
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
        public static IHtmlContent Ul(params object?[] c) => Tag("ul", c);
        public static IHtmlContent Li(params object?[] c) => Tag("li", c);
        public static IHtmlContent A(params object?[] c) => Tag("a", c);
        public static IHtmlContent Button(params object?[] c) => Tag("button", c);
        public static IHtmlContent Nav(params object?[] c) => Tag("nav", c);
        public static IHtmlContent Form(params object?[] c) => Tag("form", c);
        public static IHtmlContent Label(params object?[] c) => Tag("label", c);
        public static IHtmlContent I(params object?[] c) => Tag("i", c);
        public static IHtmlContent Code(params object?[] c) => Tag("code", c);
        public static IHtmlContent Pre(params object?[] c) => Tag("pre", c);
        public static IHtmlContent CodeBlock(string language, params object?[] c)
            => Tag("pre",
                Tag("code",
                    Class($"language-{language}"),
                    c
                )
            );

        // Table tags
        public static IHtmlContent TableHead(params object?[] c) => Tag("thead", c);
        public static IHtmlContent TableBody(params object?[] c) => Tag("tbody", c);
        public static IHtmlContent TableFoot(params object?[] c) => Tag("tfoot", c);
        public static IHtmlContent TableRow(params object?[] c) => Tag("tr", c);
        public static IHtmlContent TableHeaderCell(params object?[] c) => Tag("th", c);
        public static IHtmlContent TableDataCell(params object?[] c) => Tag("td", c);
        public static IHtmlContent Caption(params object?[] c) => Tag("caption", c);

        public static IHtmlContent Script(params object?[] c) => Tag("script", c);
        public static IHtmlContent NoScript(params object?[] c) => Tag("noscript", c);
        public static IHtmlContent Template(params object?[] c) => Tag("template", c);

        // Void tags
        public static IHtmlContent Br(params object?[] c) => VoidTag("br", c);
        public static IHtmlContent Hr(params object?[] c) => VoidTag("hr", c);
        public static IHtmlContent Img(params object?[] c) => VoidTag("img", c);
        public static IHtmlContent Input(params object?[] c) => VoidTag("input", c);
        public static IHtmlContent Meta(params object?[] c) => VoidTag("meta", c);
        public static IHtmlContent Link(params object?[] c) => VoidTag("link", c);

        // actual root tags
        public static IHtmlContent HtmlTag(params object?[] c) => Tag("html", c);
        public static IHtmlContent Head(params object?[] c) => Tag("head", c);
        public static IHtmlContent Body(params object?[] c) => Tag("body", c);
        public static IHtmlContent Footer(params object?[] c) => Tag("footer", c);


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

        /// <summary>
        /// A single HTML attribute.
        /// - Normal/Class: name="value" (value is encoded)
        /// - Boolean: name (presence-only; no ="" emitted)
        /// </summary>
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
        /// Flattens nested arrays/enumerables produced by helpers like Html.When(condition, params ...).
        /// IMPORTANT:
        /// - Treat IHtmlContent as atomic (do NOT enumerate it)
        /// - Treat string as atomic (string is IEnumerable<char>)
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

            /// <summary>
            /// Splits the provided parts into attributes and children.
            ///
            /// Enhancements included:
            /// 1) No Dictionary: attributes are merged/overwritten via linear scan (fast for small N).
            /// 2) Stable ordering: first-seen attribute position is preserved (overwrite updates in-place).
            /// 3) Boolean attrs: written as presence-only (handled in HtmlAttr.WriteTo).
            /// 4) Lower GC: attributes/children are accumulated into ArrayPool-backed buffers.
            /// 5) NEW: Flattens nested arrays/enumerables so conditional helpers can return arrays of attrs.
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

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                writer.Write('<');
                writer.Write(_name);

                // Stable attribute order.
                foreach (var attr in _attrs)
                    attr.WriteTo(writer, encoder);

                writer.Write('>');

                if (!_isVoid)
                {
                    foreach (var child in _children)
                        RenderPart(writer, encoder, child);

                    writer.Write("</");
                    writer.Write(_name);
                    writer.Write('>');
                }
            }
        }

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
