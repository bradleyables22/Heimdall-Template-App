

namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Shadow
        {
            public const string None = "shadow-none";
            public const string Sm = "shadow-sm";
            public const string Default = "shadow";
            public const string Lg = "shadow-lg";

            public static string Kind(ShadowKind k) => k switch
            {
                ShadowKind.None => None,
                ShadowKind.Sm => Sm,
                ShadowKind.Default => Default,
                ShadowKind.Lg => Lg,
                _ => throw new ArgumentOutOfRangeException(nameof(k))
            };
        }
    }
}
