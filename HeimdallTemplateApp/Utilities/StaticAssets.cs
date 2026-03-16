using Microsoft.AspNetCore.Html;
using System.Collections.Concurrent;
using System.Text.Encodings.Web;

namespace HeimdallTemplateApp.Utilities
{
    public static class StaticAssets
    {
        private static readonly ConcurrentDictionary<string, IHtmlContent> _cache = new();
        private static readonly Dictionary<string, string> _paths = new(StringComparer.OrdinalIgnoreCase);

        public static void Discover(string root)
        {
            foreach (var file in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(root, file)
                    .Replace('\\', '/');

                _paths[relative] = file;
            }
        }

        public static IHtmlContent Get(string key)
        {
            return _cache.GetOrAdd(key, Load);
        }

        private static IHtmlContent Load(string key)
        {
            if (!_paths.TryGetValue(key, out var path))
                return HtmlString.Empty;

            var markup = File.ReadAllText(path);
            return new TrustedMarkup(markup);
        }

        private sealed class TrustedMarkup : IHtmlContent
        {
            private readonly string _markup;

            public TrustedMarkup(string markup)
            {
                _markup = markup;
            }

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                writer.Write(_markup);
            }
        }
    }
}
