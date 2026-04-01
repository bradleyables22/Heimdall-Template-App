namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Table
        {
            public const string Base = "table";
            public const string Sm = "table-sm";
            public const string Striped = "table-striped";
            public const string StripedColumns = "table-striped-columns";
            public const string Hover = "table-hover";
            public const string Bordered = "table-bordered";
            public const string Borderless = "table-borderless";
            public const string Active = "table-active";
            public const string Responsive = "table-responsive";

            public static string ResponsiveAt(Breakpoint bp)
            {
                if (bp == Breakpoint.None)
                    return Responsive;

                return $"table-responsive-{Bp(bp)}";
            }

            public static string Variant(Color c) => c switch
            {
                Color.Primary => "table-primary",
                Color.Secondary => "table-secondary",
                Color.Success => "table-success",
                Color.Danger => "table-danger",
                Color.Warning => "table-warning",
                Color.Info => "table-info",
                Color.Light => "table-light",
                Color.Dark => "table-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c), $"Color '{c}' is not supported for table variant.")
            };
        }
    }
}
