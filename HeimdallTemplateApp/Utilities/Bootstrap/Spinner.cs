namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Spinner
        {
            public const string Border = "spinner-border";
            public const string Grow = "spinner-grow";
            public const string Sm = "spinner-border spinner-border-sm";
            public const string GrowSm = "spinner-grow spinner-grow-sm";

            public static string Variant(Color c) => c switch
            {
                Color.Primary => "text-primary",
                Color.Secondary => "text-secondary",
                Color.Success => "text-success",
                Color.Danger => "text-danger",
                Color.Warning => "text-warning",
                Color.Info => "text-info",
                Color.Light => "text-light",
                Color.Dark => "text-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c), $"Color '{c}' is not supported for spinner variant.")
            };
        }
    }
}
