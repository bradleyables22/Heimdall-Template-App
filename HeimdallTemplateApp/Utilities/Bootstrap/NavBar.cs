
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Navbar
        {
            public const string Base = "navbar";
            public const string Brand = "navbar-brand";
            public const string Toggler = "navbar-toggler";
            public const string TogglerIcon = "navbar-toggler-icon";
            public const string Nav = "navbar-nav";
            public const string Text = "navbar-text";
            public const string Collapse = "navbar-collapse";

            public static string Expand(Breakpoint bp)
            {
                if (bp == Breakpoint.None) return "navbar-expand";
                return $"navbar-expand-{Bp(bp)}";
            }

            public const string Light = "navbar-light";
            public const string Dark = "navbar-dark";
        }
    }
}
