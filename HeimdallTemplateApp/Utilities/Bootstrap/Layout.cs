
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Layout
        {
            public const string Container = "container";
            public const string ContainerFluid = "container-fluid";
            public const string ContainerSm = "container-sm";
            public const string ContainerMd = "container-md";
            public const string ContainerLg = "container-lg";
            public const string ContainerXl = "container-xl";
            public const string ContainerXxl = "container-xxl";

            public const string Row = "row";
            public const string Col = "col";
            public const string ColAuto = "col-auto";

            

            public static string ColSpan(int span, Breakpoint bp = Breakpoint.None)
            {
                if (span < 1 || span > 12)
                    throw new ArgumentOutOfRangeException(nameof(span), "Bootstrap columns are 1..12.");

                return bp == Breakpoint.None
                    ? $"col-{span}"
                    : $"col-{Bp(bp)}-{span}";
            }

            public static string ColAutoAt(Breakpoint bp)
            {
                if (bp == Breakpoint.None)
                    return ColAuto;

                return $"col-{Bp(bp)}-auto";
            }

            public static string RowCols(int cols, Breakpoint bp = Breakpoint.None)
            {
                if (cols < 1 || cols > 6)
                    throw new ArgumentOutOfRangeException(nameof(cols), "Bootstrap row-cols are typically 1..6.");

                return bp == Breakpoint.None
                    ? $"row-cols-{cols}"
                    : $"row-cols-{Bp(bp)}-{cols}";
            }

            public static string Offset(int span, Breakpoint bp = Breakpoint.None)
            {
                if (span < 0 || span > 11)
                    throw new ArgumentOutOfRangeException(nameof(span), "Bootstrap offsets are 0..11.");

                return bp == Breakpoint.None
                    ? $"offset-{span}"
                    : $"offset-{Bp(bp)}-{span}";
            }

            public static string Gutter(int n, Breakpoint bp = Breakpoint.None)
                => GutterInternal("g", n, bp);

            public static string GutterX(int n, Breakpoint bp = Breakpoint.None)
                => GutterInternal("gx", n, bp);

            public static string GutterY(int n, Breakpoint bp = Breakpoint.None)
                => GutterInternal("gy", n, bp);

            private static string GutterInternal(string prefix, int n, Breakpoint bp)
            {
                if (n < 0 || n > 5)
                    throw new ArgumentOutOfRangeException(nameof(n), "Bootstrap gutter scale is 0..5.");

                return bp == Breakpoint.None
                    ? $"{prefix}-{n}"
                    : $"{prefix}-{Bp(bp)}-{n}";
            }
        }
    }
}
