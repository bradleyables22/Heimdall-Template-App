
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Overflow
        {
            public static string All(OverflowKind kind) => $"overflow-{OverflowToken(kind)}";
            public static string X(OverflowKind kind) => $"overflow-x-{OverflowToken(kind)}";
            public static string Y(OverflowKind kind) => $"overflow-y-{OverflowToken(kind)}";

            private static string OverflowToken(OverflowKind k) => k switch
            {
                OverflowKind.Auto => "auto",
                OverflowKind.Hidden => "hidden",
                OverflowKind.Visible => "visible",
                OverflowKind.Scroll => "scroll",
                _ => throw new ArgumentOutOfRangeException(nameof(k))
            };
        }
    }
}
