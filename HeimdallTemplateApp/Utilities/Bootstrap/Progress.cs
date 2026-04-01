namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Progress
        {
            public const string Base = "progress";
            public const string Bar = "progress-bar";
            public const string Striped = "progress-bar-striped";
            public const string Animated = "progress-bar-animated";

            public static string Variant(Color c) => c switch
            {
                Color.Primary => "bg-primary",
                Color.Secondary => "bg-secondary",
                Color.Success => "bg-success",
                Color.Danger => "bg-danger",
                Color.Warning => "bg-warning",
                Color.Info => "bg-info",
                Color.Light => "bg-light",
                Color.Dark => "bg-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c), $"Color '{c}' is not supported for progress variant.")
            };
        }
    }
}
