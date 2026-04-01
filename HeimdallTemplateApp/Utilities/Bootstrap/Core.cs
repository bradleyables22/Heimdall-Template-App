namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {

        public static string Css(params string?[] parts)
            => CssInternal(dedupe: true, parts);

        public static string CssOrdered(params string?[] parts)
            => CssInternal(dedupe: false, parts);

        private static string CssInternal(bool dedupe, params string?[] parts)
        {
            if (parts is null || parts.Length == 0)
                return string.Empty;

            var ordered = new List<string>(64);

            if (!dedupe)
            {
                foreach (var p in parts)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;

                    ordered.AddRange(
                        p.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }

                return string.Join(' ', ordered);
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var p in parts)
            {
                if (string.IsNullOrWhiteSpace(p)) continue;

                foreach (var t in p.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (seen.Add(t))
                        ordered.Add(t);
                }
            }

            return string.Join(' ', ordered);
        }

        public static string Raw(string cssTokens) => cssTokens?.Trim() ?? string.Empty;

        public enum Breakpoint { None, Sm, Md, Lg, Xl, Xxl }
        public enum Side { None, Top, Bottom, Start, End, X, Y }
        public enum DisplayKind { None, Inline, InlineBlock, Block, Grid, InlineGrid, Flex, InlineFlex, Table, TableRow, TableCell }
        public enum PositionKind { Static, Relative, Absolute, Fixed, Sticky }
        public enum TextAlignKind { Start, Center, End }
        public enum FloatKind { Start, End, None }
        public enum OverflowKind { Auto, Hidden, Visible, Scroll }
        public enum AlignItemsKind { Start, End, Center, Baseline, Stretch }
        public enum AlignSelfKind { Auto, Start, End, Center, Baseline, Stretch }
        public enum JustifyContentKind { Start, End, Center, Between, Around, Evenly }
        public enum FlexDirectionKind { Row, RowReverse, Column, ColumnReverse }
        public enum FlexWrapKind { Wrap, Nowrap, WrapReverse }
        public enum TextTransformKind { Lowercase, Uppercase, Capitalize }
        public enum FontWeightKind { Light, Lighter, Normal, Medium, Semibold, Bold, Bolder }
        public enum LineHeightKind { One, Sm, Base, Lg }
        public enum RoundedKind { None, Sm, Default, Lg, Xl, Xxl, Pill, Circle }
        public enum ShadowKind { None, Sm, Default, Lg }
        public enum Placement { Top, Bottom, Start, End }
        public enum RatioKind { R1x1, R4x3, R16x9, R21x9 }
        public enum FontSizeKind { Fs1, Fs2, Fs3, Fs4, Fs5, Fs6 }
        public enum DisplaySizeKind { Display1, Display2, Display3, Display4, Display5, Display6 }

        public enum Color
        {
            Primary,
            Secondary,
            Success,
            Danger,
            Warning,
            Info,
            Light,
            Dark,

            // Theme/body helpers
            Body,
            BodySecondary,
            BodyTertiary,

            // Only valid for a few helpers (e.g. bg-transparent)
            Transparent
        }
        private static string Bp(Breakpoint bp) => bp switch
        {
            Breakpoint.None => "",
            Breakpoint.Sm => "sm",
            Breakpoint.Md => "md",
            Breakpoint.Lg => "lg",
            Breakpoint.Xl => "xl",
            Breakpoint.Xxl => "xxl",
            _ => throw new ArgumentOutOfRangeException(nameof(bp))
        };
    }
}
