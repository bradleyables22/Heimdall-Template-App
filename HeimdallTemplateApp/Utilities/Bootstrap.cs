
using System.Runtime.ConstrainedExecution;

namespace HeimdallTemplateApp.Utilities
{
    public static class Bootstrap
    {
        /// <summary>
        /// Combine CSS tokens; trims, removes empties, and de-dupes tokens.
        /// Keeps original order only loosely (HashSet-based). If strict ordering matters, use CssOrdered.
        /// </summary>
        public static string Css(params string?[] parts)
            => CssInternal(dedupe: true, parts);

        /// <summary>
        /// Combine CSS tokens; trims, removes empties, preserves token order, no de-dupe.
        /// </summary>
        public static string CssOrdered(params string?[] parts)
            => CssInternal(dedupe: false, parts);

        private static string CssInternal(bool dedupe, params string?[] parts)
        {
            if (parts is null || parts.Length == 0) return string.Empty;

            if (!dedupe)
            {
                var ordered = new List<string>(64);
                foreach (var p in parts)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;
                    ordered.AddRange(p.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }
                return string.Join(' ', ordered);
            }

            var tokens = new HashSet<string>(StringComparer.Ordinal);
            foreach (var p in parts)
            {
                if (string.IsNullOrWhiteSpace(p)) continue;
                foreach (var t in p.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    tokens.Add(t);
            }
            return string.Join(' ', tokens);
        }

        /// <summary>Escape hatch for any class tokens not covered by helpers.</summary>
        public static string Raw(string cssTokens) => cssTokens?.Trim() ?? string.Empty;

        // --------------------------------------------------------------------
        // Common enums
        // --------------------------------------------------------------------

        public enum Breakpoint { None, Sm, Md, Lg, Xl, Xxl }
        public enum Side { None, Top, Bottom, Start, End, X, Y }
        public enum DisplayKind { None, Inline, InlineBlock, Block, Grid, InlineGrid, Flex, InlineFlex, Table, TableRow, TableCell, NoneDisplay }
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

        public enum Color
        {
            Primary, Secondary, Success, Danger, Warning, Info, Light, Dark,
            // not part of the 8 semantic colors, but common helpers below can use these:
            Body, BodySecondary, BodyTertiary, Transparent
        }

        // --------------------------------------------------------------------
        // Layout / grid / containers
        // --------------------------------------------------------------------

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
                if (span < 1 || span > 12) throw new ArgumentOutOfRangeException(nameof(span), "Bootstrap columns are 1..12.");
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"col-{span}" : $"col-{b}-{span}";
            }

            public static string ColAutoAt(Breakpoint bp)
            {
                var b = Bp(bp);
                return $"col-{b}-auto";
            }

            public static string RowCols(int cols, Breakpoint bp = Breakpoint.None)
            {
                if (cols < 1 || cols > 6) throw new ArgumentOutOfRangeException(nameof(cols), "Bootstrap row-cols are typically 1..6.");
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"row-cols-{cols}" : $"row-cols-{b}-{cols}";
            }

            public static string Offset(int span, Breakpoint bp = Breakpoint.None)
            {
                if (span < 0 || span > 11) throw new ArgumentOutOfRangeException(nameof(span), "Bootstrap offsets are 0..11.");
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"offset-{span}" : $"offset-{b}-{span}";
            }

            public static string Gutter(int n, Breakpoint bp = Breakpoint.None)
                => GutterInternal("g", n, bp);

            public static string GutterX(int n, Breakpoint bp = Breakpoint.None)
                => GutterInternal("gx", n, bp);

            public static string GutterY(int n, Breakpoint bp = Breakpoint.None)
                => GutterInternal("gy", n, bp);

            private static string GutterInternal(string prefix, int n, Breakpoint bp)
            {
                if (n < 0 || n > 5) throw new ArgumentOutOfRangeException(nameof(n), "Bootstrap gutter scale is 0..5.");
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"{prefix}-{n}" : $"{prefix}-{b}-{n}";
            }
        }

        // --------------------------------------------------------------------
        // Display / positioning / sizing / overflow / visibility / opacity / z-index
        // --------------------------------------------------------------------

        public static class Display
        {
            public const string Block = "d-block";
            public const string Inline = "d-inline";
            public const string InlineBlock = "d-inline-block";
            public const string Grid = "d-grid";
            public const string InlineGrid = "d-inline-grid";
            public const string Flex = "d-flex";
            public const string InlineFlex = "d-inline-flex";
            public const string Table = "d-table";
            public const string TableRow = "d-table-row";
            public const string TableCell = "d-table-cell";
            public const string None = "d-none";

            public static string At(Breakpoint bp, DisplayKind kind)
            {
                var b = Bp(bp);
                var k = kind switch
                {
                    DisplayKind.Inline => "inline",
                    DisplayKind.InlineBlock => "inline-block",
                    DisplayKind.Block => "block",
                    DisplayKind.Grid => "grid",
                    DisplayKind.InlineGrid => "inline-grid",
                    DisplayKind.Flex => "flex",
                    DisplayKind.InlineFlex => "inline-flex",
                    DisplayKind.Table => "table",
                    DisplayKind.TableRow => "table-row",
                    DisplayKind.TableCell => "table-cell",
                    DisplayKind.NoneDisplay => "none",
                    _ => throw new ArgumentOutOfRangeException(nameof(kind))
                };
                return $"d-{b}-{k}";
            }
        }

        public static class Position
        {
            public const string Static = "position-static";
            public const string Relative = "position-relative";
            public const string Absolute = "position-absolute";
            public const string Fixed = "position-fixed";
            public const string Sticky = "position-sticky";

            public static string Kind(PositionKind kind) => kind switch
            {
                PositionKind.Static => Static,
                PositionKind.Relative => Relative,
                PositionKind.Absolute => Absolute,
                PositionKind.Fixed => Fixed,
                PositionKind.Sticky => Sticky,
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            // Placement helpers
            public const string Top0 = "top-0";
            public const string Top50 = "top-50";
            public const string Top100 = "top-100";
            public const string Bottom0 = "bottom-0";
            public const string Bottom50 = "bottom-50";
            public const string Bottom100 = "bottom-100";
            public const string Start0 = "start-0";
            public const string Start50 = "start-50";
            public const string Start100 = "start-100";
            public const string End0 = "end-0";
            public const string End50 = "end-50";
            public const string End100 = "end-100";

            public const string TranslateMiddle = "translate-middle";
            public const string TranslateMiddleX = "translate-middle-x";
            public const string TranslateMiddleY = "translate-middle-y";
        }

        public static class Sizing
        {
            // width
            public const string W25 = "w-25";
            public const string W50 = "w-50";
            public const string W75 = "w-75";
            public const string W100 = "w-100";
            public const string WAuto = "w-auto";

            // height
            public const string H25 = "h-25";
            public const string H50 = "h-50";
            public const string H75 = "h-75";
            public const string H100 = "h-100";
            public const string HAuto = "h-auto";

            // viewport sizing
            public const string Vw100 = "vw-100";
            public const string Vh100 = "vh-100";
            public const string MinVw100 = "min-vw-100";
            public const string MinVh100 = "min-vh-100";

            // max sizing
            public const string Mw100 = "mw-100";
            public const string Mh100 = "mh-100";
        }

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

        public static class Visibility
        {
            public const string Visible = "visible";
            public const string Invisible = "invisible";

            public const string VisuallyHidden = "visually-hidden";
            public const string VisuallyHiddenFocusable = "visually-hidden-focusable";
        }

        public static class Opacity
        {
            public const string Opacity0 = "opacity-0";
            public const string Opacity25 = "opacity-25";
            public const string Opacity50 = "opacity-50";
            public const string Opacity75 = "opacity-75";
            public const string Opacity100 = "opacity-100";
        }

        public static class ZIndex
        {
            public const string N1 = "z-n1";
            public const string Z0 = "z-0";
            public const string Z1 = "z-1";
            public const string Z2 = "z-2";
            public const string Z3 = "z-3";
        }

        // --------------------------------------------------------------------
        // Spacing
        // --------------------------------------------------------------------

        public static class Spacing
        {
            // margin
            public static string M(int n, Side side = Side.None, Breakpoint bp = Breakpoint.None) => Space("m", n, side, bp);
            public static string Mx(int n, Breakpoint bp = Breakpoint.None) => Space("m", n, Side.X, bp);
            public static string My(int n, Breakpoint bp = Breakpoint.None) => Space("m", n, Side.Y, bp);
            public static string Mt(int n, Breakpoint bp = Breakpoint.None) => Space("m", n, Side.Top, bp);
            public static string Mb(int n, Breakpoint bp = Breakpoint.None) => Space("m", n, Side.Bottom, bp);
            public static string Ms(int n, Breakpoint bp = Breakpoint.None) => Space("m", n, Side.Start, bp);
            public static string Me(int n, Breakpoint bp = Breakpoint.None) => Space("m", n, Side.End, bp);

            public static string MAuto(Side side = Side.None, Breakpoint bp = Breakpoint.None) => SpaceAuto("m", side, bp);
            public static string MxAuto(Breakpoint bp = Breakpoint.None) => SpaceAuto("m", Side.X, bp);
            public static string MyAuto(Breakpoint bp = Breakpoint.None) => SpaceAuto("m", Side.Y, bp);
            public static string MtAuto(Breakpoint bp = Breakpoint.None) => SpaceAuto("m", Side.Top, bp);
            public static string MbAuto(Breakpoint bp = Breakpoint.None) => SpaceAuto("m", Side.Bottom, bp);
            public static string MsAuto(Breakpoint bp = Breakpoint.None) => SpaceAuto("m", Side.Start, bp);
            public static string MeAuto(Breakpoint bp = Breakpoint.None) => SpaceAuto("m", Side.End, bp);

            // padding
            public static string P(int n, Side side = Side.None, Breakpoint bp = Breakpoint.None) => Space("p", n, side, bp);
            public static string Px(int n, Breakpoint bp = Breakpoint.None) => Space("p", n, Side.X, bp);
            public static string Py(int n, Breakpoint bp = Breakpoint.None) => Space("p", n, Side.Y, bp);
            public static string Pt(int n, Breakpoint bp = Breakpoint.None) => Space("p", n, Side.Top, bp);
            public static string Pb(int n, Breakpoint bp = Breakpoint.None) => Space("p", n, Side.Bottom, bp);
            public static string Ps(int n, Breakpoint bp = Breakpoint.None) => Space("p", n, Side.Start, bp);
            public static string Pe(int n, Breakpoint bp = Breakpoint.None) => Space("p", n, Side.End, bp);

            // gap (0..5)
            public static string Gap(int n, Breakpoint bp = Breakpoint.None) => GapInternal("gap", n, bp);
            public static string GapX(int n, Breakpoint bp = Breakpoint.None) => GapInternal("column-gap", n, bp);
            public static string GapY(int n, Breakpoint bp = Breakpoint.None) => GapInternal("row-gap", n, bp);

            private static string GapInternal(string prefix, int n, Breakpoint bp)
            {
                if (n < 0 || n > 5) throw new ArgumentOutOfRangeException(nameof(n), "Bootstrap scale is 0..5.");
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"{prefix}-{n}" : $"{prefix}-{b}-{n}";
            }

            private static string Space(string prefix, int n, Side side, Breakpoint bp)
            {
                if (n < 0 || n > 5) throw new ArgumentOutOfRangeException(nameof(n), "Bootstrap spacing scale is 0..5.");
                var s = SideToken(side);
                var b = Bp(bp);
                if (bp == Breakpoint.None)
                    return side == Side.None ? $"{prefix}-{n}" : $"{prefix}{s}-{n}";
                return side == Side.None ? $"{prefix}-{b}-{n}" : $"{prefix}{s}-{b}-{n}";
            }

            private static string SpaceAuto(string prefix, Side side, Breakpoint bp)
            {
                var s = SideToken(side);
                var b = Bp(bp);
                if (bp == Breakpoint.None)
                    return side == Side.None ? $"{prefix}-auto" : $"{prefix}{s}-auto";
                return side == Side.None ? $"{prefix}-{b}-auto" : $"{prefix}{s}-{b}-auto";
            }

            private static string SideToken(Side side) => side switch
            {
                Side.None => "",
                Side.Top => "t",
                Side.Bottom => "b",
                Side.Start => "s",
                Side.End => "e",
                Side.X => "x",
                Side.Y => "y",
                _ => throw new ArgumentOutOfRangeException(nameof(side))
            };
        }

        // --------------------------------------------------------------------
        // Flex / alignment / order
        // --------------------------------------------------------------------

        public static class Flex
        {
            // Direction / wrap
            public const string Row = "flex-row";
            public const string RowReverse = "flex-row-reverse";
            public const string Column = "flex-column";
            public const string ColumnReverse = "flex-column-reverse";

            public const string Wrap = "flex-wrap";
            public const string Nowrap = "flex-nowrap";
            public const string WrapReverse = "flex-wrap-reverse";

            // Grow / shrink
            public const string Grow0 = "flex-grow-0";
            public const string Grow1 = "flex-grow-1";
            public const string Shrink0 = "flex-shrink-0";
            public const string Shrink1 = "flex-shrink-1";

            // Fill
            public const string Fill = "flex-fill";

            // Align items
            public const string AlignItemsStart = "align-items-start";
            public const string AlignItemsEnd = "align-items-end";
            public const string AlignItemsCenter = "align-items-center";
            public const string AlignItemsBaseline = "align-items-baseline";
            public const string AlignItemsStretch = "align-items-stretch";

            // Align self
            public const string AlignSelfAuto = "align-self-auto";
            public const string AlignSelfStart = "align-self-start";
            public const string AlignSelfEnd = "align-self-end";
            public const string AlignSelfCenter = "align-self-center";
            public const string AlignSelfBaseline = "align-self-baseline";
            public const string AlignSelfStretch = "align-self-stretch";

            // Align content
            public const string AlignContentStart = "align-content-start";
            public const string AlignContentEnd = "align-content-end";
            public const string AlignContentCenter = "align-content-center";
            public const string AlignContentBetween = "align-content-between";
            public const string AlignContentAround = "align-content-around";
            public const string AlignContentStretch = "align-content-stretch";

            // Justify content
            public const string JustifyStart = "justify-content-start";
            public const string JustifyEnd = "justify-content-end";
            public const string JustifyCenter = "justify-content-center";
            public const string JustifyBetween = "justify-content-between";
            public const string JustifyAround = "justify-content-around";
            public const string JustifyEvenly = "justify-content-evenly";

            // Order
            public const string OrderFirst = "order-first";
            public const string OrderLast = "order-last";
            public const string Order0 = "order-0";
            public const string Order1 = "order-1";
            public const string Order2 = "order-2";
            public const string Order3 = "order-3";
            public const string Order4 = "order-4";
            public const string Order5 = "order-5";

            // Gap (alias to spacing gap)
            public static string Gap(int n, Breakpoint bp = Breakpoint.None) => Spacing.Gap(n, bp);

            // Responsive variants
            public static string Direction(FlexDirectionKind dir, Breakpoint bp = Breakpoint.None)
            {
                var t = dir switch
                {
                    FlexDirectionKind.Row => "row",
                    FlexDirectionKind.RowReverse => "row-reverse",
                    FlexDirectionKind.Column => "column",
                    FlexDirectionKind.ColumnReverse => "column-reverse",
                    _ => throw new ArgumentOutOfRangeException(nameof(dir))
                };
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"flex-{t}" : $"flex-{b}-{t}";
            }

            public static string WrapMode(FlexWrapKind wrap, Breakpoint bp = Breakpoint.None)
            {
                var t = wrap switch
                {
                    FlexWrapKind.Wrap => "wrap",
                    FlexWrapKind.Nowrap => "nowrap",
                    FlexWrapKind.WrapReverse => "wrap-reverse",
                    _ => throw new ArgumentOutOfRangeException(nameof(wrap))
                };
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"flex-{t}" : $"flex-{b}-{t}";
            }

            public static string Justify(JustifyContentKind justify, Breakpoint bp = Breakpoint.None)
            {
                var t = justify switch
                {
                    JustifyContentKind.Start => "start",
                    JustifyContentKind.End => "end",
                    JustifyContentKind.Center => "center",
                    JustifyContentKind.Between => "between",
                    JustifyContentKind.Around => "around",
                    JustifyContentKind.Evenly => "evenly",
                    _ => throw new ArgumentOutOfRangeException(nameof(justify))
                };
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"justify-content-{t}" : $"justify-content-{b}-{t}";
            }

            public static string AlignItems(AlignItemsKind align, Breakpoint bp = Breakpoint.None)
            {
                var t = align switch
                {
                    AlignItemsKind.Start => "start",
                    AlignItemsKind.End => "end",
                    AlignItemsKind.Center => "center",
                    AlignItemsKind.Baseline => "baseline",
                    AlignItemsKind.Stretch => "stretch",
                    _ => throw new ArgumentOutOfRangeException(nameof(align))
                };
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"align-items-{t}" : $"align-items-{b}-{t}";
            }

            public static string AlignSelf(AlignSelfKind align, Breakpoint bp = Breakpoint.None)
            {
                var t = align switch
                {
                    AlignSelfKind.Auto => "auto",
                    AlignSelfKind.Start => "start",
                    AlignSelfKind.End => "end",
                    AlignSelfKind.Center => "center",
                    AlignSelfKind.Baseline => "baseline",
                    AlignSelfKind.Stretch => "stretch",
                    _ => throw new ArgumentOutOfRangeException(nameof(align))
                };
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"align-self-{t}" : $"align-self-{b}-{t}";
            }
        }

        // --------------------------------------------------------------------
        // Text / links / typography
        // --------------------------------------------------------------------

        public static class Text
        {
            // Alignment
            public const string Start = "text-start";
            public const string Center = "text-center";
            public const string End = "text-end";

            public static string Align(TextAlignKind align, Breakpoint bp = Breakpoint.None)
            {
                var t = align switch
                {
                    TextAlignKind.Start => "start",
                    TextAlignKind.Center => "center",
                    TextAlignKind.End => "end",
                    _ => throw new ArgumentOutOfRangeException(nameof(align))
                };
                var b = Bp(bp);
                return bp == Breakpoint.None ? $"text-{t}" : $"text-{b}-{t}";
            }

            // Wrapping / breaking
            public const string Wrap = "text-wrap";
            public const string Nowrap = "text-nowrap";
            public const string Break = "text-break";
            public const string Truncate = "text-truncate";

            // Transform
            public const string Lowercase = "text-lowercase";
            public const string Uppercase = "text-uppercase";
            public const string Capitalize = "text-capitalize";

            public static string Transform(TextTransformKind t) => t switch
            {
                TextTransformKind.Lowercase => Lowercase,
                TextTransformKind.Uppercase => Uppercase,
                TextTransformKind.Capitalize => Capitalize,
                _ => throw new ArgumentOutOfRangeException(nameof(t))
            };

            // Weight / style
            public const string FwLight = "fw-light";
            public const string FwLighter = "fw-lighter";
            public const string FwNormal = "fw-normal";
            public const string FwMedium = "fw-medium";
            public const string FwSemibold = "fw-semibold";
            public const string FwBold = "fw-bold";
            public const string FwBolder = "fw-bolder";

            public const string FstItalic = "fst-italic";
            public const string FstNormal = "fst-normal";

            public static string Weight(FontWeightKind w) => w switch
            {
                FontWeightKind.Light => FwLight,
                FontWeightKind.Lighter => FwLighter,
                FontWeightKind.Normal => FwNormal,
                FontWeightKind.Medium => FwMedium,
                FontWeightKind.Semibold => FwSemibold,
                FontWeightKind.Bold => FwBold,
                FontWeightKind.Bolder => FwBolder,
                _ => throw new ArgumentOutOfRangeException(nameof(w))
            };

            // Line-height
            public const string Lh1 = "lh-1";
            public const string LhSm = "lh-sm";
            public const string LhBase = "lh-base";
            public const string LhLg = "lh-lg";

            public static string LineHeight(LineHeightKind lh) => lh switch
            {
                LineHeightKind.One => Lh1,
                LineHeightKind.Sm => LhSm,
                LineHeightKind.Base => LhBase,
                LineHeightKind.Lg => LhLg,
                _ => throw new ArgumentOutOfRangeException(nameof(lh))
            };

            // Misc typography
            public const string Lead = "lead";
            public const string Small = "small";
            public const string Mark = "mark";
            public const string Monospace = "font-monospace";

            // Colors (text-*)
            public const string Muted = "text-muted"; // still present, but prefer body-secondary in BS 5.3
            public const string Body = "text-body";
            public const string BodySecondary = "text-body-secondary";
            public const string BodyTertiary = "text-body-tertiary";
            public const string Emphasis = "text-body-emphasis";
            public const string Reset = "text-reset";
            public const string White = "text-white";
            public const string Black = "text-black";
            public const string Black50 = "text-black-50";
            public const string White50 = "text-white-50";

            public static string TxtColor(Color c) => c switch
            {
                Color.Primary => "text-primary",
                Color.Secondary => "text-secondary",
                Color.Success => "text-success",
                Color.Danger => "text-danger",
                Color.Warning => "text-warning",
                Color.Info => "text-info",
                Color.Light => "text-light",
                Color.Dark => "text-dark",
                Color.Body => Body,
                Color.BodySecondary => BodySecondary,
                Color.BodyTertiary => BodyTertiary,
                Color.Transparent => "text-transparent", // not real
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };

            // Opacity (text-opacity-*)
            public const string Opacity25 = "text-opacity-25";
            public const string Opacity50 = "text-opacity-50";
            public const string Opacity75 = "text-opacity-75";
            public const string Opacity100 = "text-opacity-100";
            public enum TextOpacity
            {
                Opacity25,
                Opacity50,
                Opacity75,
                Opacity100
            }

            public static string Opacity(TextOpacity o) => o switch
            {
                TextOpacity.Opacity25 => "text-opacity-25",
                TextOpacity.Opacity50 => "text-opacity-50",
                TextOpacity.Opacity75 => "text-opacity-75",
                TextOpacity.Opacity100 => "text-opacity-100",
                _ => throw new ArgumentOutOfRangeException(nameof(o))
            };

            // Decorations
            public const string DecorationNone = "text-decoration-none";
            public const string DecorationUnderline = "text-decoration-underline";
            public const string DecorationLineThrough = "text-decoration-line-through";

            // Links
            public const string LinkOpacity10 = "link-opacity-10";
            public const string LinkOpacity25 = "link-opacity-25";
            public const string LinkOpacity50 = "link-opacity-50";
            public const string LinkOpacity75 = "link-opacity-75";
            public const string LinkOpacity100 = "link-opacity-100";

            public const string LinkUnderline = "link-underline";
            public const string LinkUnderlineOpacity0 = "link-underline-opacity-0";
            public const string LinkUnderlineOpacity10 = "link-underline-opacity-10";
            public const string LinkUnderlineOpacity25 = "link-underline-opacity-25";
            public const string LinkUnderlineOpacity50 = "link-underline-opacity-50";
            public const string LinkUnderlineOpacity75 = "link-underline-opacity-75";
            public const string LinkUnderlineOpacity100 = "link-underline-opacity-100";

            public static string LinkColor(Color c) => c switch
            {
                Color.Primary => "link-primary",
                Color.Secondary => "link-secondary",
                Color.Success => "link-success",
                Color.Danger => "link-danger",
                Color.Warning => "link-warning",
                Color.Info => "link-info",
                Color.Light => "link-light",
                Color.Dark => "link-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        //sizing
        public const string Fs1 = "fs-1";
        public const string Fs2 = "fs-2";
        public const string Fs3 = "fs-3";
        public const string Fs4 = "fs-4";
        public const string Fs5 = "fs-5";
        public const string Fs6 = "fs-6";

        public enum FontSizeKind
        {
            Fs1,
            Fs2,
            Fs3,
            Fs4,
            Fs5,
            Fs6
        }
        public static string Size(FontSizeKind size) => size switch
        {
            FontSizeKind.Fs1 => "fs-1",
            FontSizeKind.Fs2 => "fs-2",
            FontSizeKind.Fs3 => "fs-3",
            FontSizeKind.Fs4 => "fs-4",
            FontSizeKind.Fs5 => "fs-5",
            FontSizeKind.Fs6 => "fs-6",
            _ => throw new ArgumentOutOfRangeException(nameof(size))
        };

        public const string Display1 = "display-1";
        public const string Display2 = "display-2";
        public const string Display3 = "display-3";
        public const string Display4 = "display-4";
        public const string Display5 = "display-5";
        public const string Display6 = "display-6";

        public enum DisplaySizeKind
        {
            Display1,
            Display2,
            Display3,
            Display4,
            Display5,
            Display6
        }
        public static string DisplaySize(DisplaySizeKind size) => size switch
        {
            DisplaySizeKind.Display1 => "display-1",
            DisplaySizeKind.Display2 => "display-2",
            DisplaySizeKind.Display3 => "display-3",
            DisplaySizeKind.Display4 => "display-4",
            DisplaySizeKind.Display5 => "display-5",
            DisplaySizeKind.Display6 => "display-6",
            _ => throw new ArgumentOutOfRangeException(nameof(size))
        };
        public static string Emphasis(Color c) => c switch
        {
            Color.Primary => "text-primary-emphasis",
            Color.Secondary => "text-secondary-emphasis",
            Color.Success => "text-success-emphasis",
            Color.Danger => "text-danger-emphasis",
            Color.Warning => "text-warning-emphasis",
            Color.Info => "text-info-emphasis",
            Color.Light => "text-light-emphasis",
            Color.Dark => "text-dark-emphasis",
            _ => throw new ArgumentOutOfRangeException(nameof(c))
        };
        // --------------------------------------------------------------------
        // Backgrounds
        // --------------------------------------------------------------------

        public static class Bg
        {
            public const string Body = "bg-body";
            public const string BodySecondary = "bg-body-secondary";
            public const string BodyTertiary = "bg-body-tertiary";
            public const string Transparent = "bg-transparent";

            public static string BgColor(Color c) => c switch
            {
                Color.Primary => "bg-primary",
                Color.Secondary => "bg-secondary",
                Color.Success => "bg-success",
                Color.Danger => "bg-danger",
                Color.Warning => "bg-warning",
                Color.Info => "bg-info",
                Color.Light => "bg-light",
                Color.Dark => "bg-dark",
                Color.Body => Body,
                Color.BodySecondary => BodySecondary,
                Color.BodyTertiary => BodyTertiary,
                Color.Transparent => Transparent,
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };

            public static string Subtle(Color c) => c switch
            {
                Color.Primary => "bg-primary-subtle",
                Color.Secondary => "bg-secondary-subtle",
                Color.Success => "bg-success-subtle",
                Color.Danger => "bg-danger-subtle",
                Color.Warning => "bg-warning-subtle",
                Color.Info => "bg-info-subtle",
                Color.Light => "bg-light-subtle",
                Color.Dark => "bg-dark-subtle",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };

            public const string Opacity10 = "bg-opacity-10";
            public const string Opacity25 = "bg-opacity-25";
            public const string Opacity50 = "bg-opacity-50";
            public const string Opacity75 = "bg-opacity-75";
            public const string Opacity100 = "bg-opacity-100";

            public static string TextBg(Color c) => c switch
            {
                Color.Primary => "text-bg-primary",
                Color.Secondary => "text-bg-secondary",
                Color.Success => "text-bg-success",
                Color.Danger => "text-bg-danger",
                Color.Warning => "text-bg-warning",
                Color.Info => "text-bg-info",
                Color.Light => "text-bg-light",
                Color.Dark => "text-bg-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        // --------------------------------------------------------------------
        // Borders / rounding / shadows
        // --------------------------------------------------------------------

        public static class Border
        {
            public const string Default = "border";
            public const string None = "border-0";
            public const string Top = "border-top";
            public const string Top0 = "border-top-0";
            public const string Bottom = "border-bottom";
            public const string Bottom0 = "border-bottom-0";
            public const string Start = "border-start";
            public const string Start0 = "border-start-0";
            public const string End = "border-end";
            public const string End0 = "border-end-0";

            public const string Width1 = "border-1";
            public const string Width2 = "border-2";
            public const string Width3 = "border-3";
            public const string Width4 = "border-4";
            public const string Width5 = "border-5";

            public static string BorderColor(Color c) => c switch
            {
                Color.Primary => "border-primary",
                Color.Secondary => "border-secondary",
                Color.Success => "border-success",
                Color.Danger => "border-danger",
                Color.Warning => "border-warning",
                Color.Info => "border-info",
                Color.Light => "border-light",
                Color.Dark => "border-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };

            public const string Opacity10 = "border-opacity-10";
            public const string Opacity25 = "border-opacity-25";
            public const string Opacity50 = "border-opacity-50";
            public const string Opacity75 = "border-opacity-75";
            public const string Opacity100 = "border-opacity-100";

            public const string Rounded0 = "rounded-0";
            public const string Rounded1 = "rounded-1";
            public const string Rounded2 = "rounded-2";
            public const string Rounded3 = "rounded-3";
            public const string Rounded4 = "rounded-4";
            public const string Rounded5 = "rounded-5";
            public const string Rounded = "rounded";

            public const string RoundedTop = "rounded-top";
            public const string RoundedBottom = "rounded-bottom";
            public const string RoundedStart = "rounded-start";
            public const string RoundedEnd = "rounded-end";

            public const string RoundedTop0 = "rounded-top-0";
            public const string RoundedBottom0 = "rounded-bottom-0";
            public const string RoundedStart0 = "rounded-start-0";
            public const string RoundedEnd0 = "rounded-end-0";

            public static string Round(int n)
            {
                if (n < 0 || n > 5) 
                    throw new ArgumentOutOfRangeException(nameof(n));
                return n == 0 ? "rounded-0" : $"rounded-{n}";
            }
        }

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

        public static class Float
        {
            public const string Start = "float-start";
            public const string End = "float-end";
            public const string None = "float-none";
            public const string Clearfix = "clearfix";

            public static string At(Breakpoint bp, FloatKind kind)
            {
                var b = Bp(bp);
                var k = kind switch
                {
                    FloatKind.Start => "start",
                    FloatKind.End => "end",
                    FloatKind.None => "none",
                    _ => throw new ArgumentOutOfRangeException(nameof(kind))
                };
                return $"float-{b}-{k}";
            }
        }

        public static class Interaction
        {
            public const string UserSelectAll = "user-select-all";
            public const string UserSelectAuto = "user-select-auto";
            public const string UserSelectNone = "user-select-none";

            public const string PeNone = "pe-none";
            public const string PeAuto = "pe-auto";

            public const string CursorPointer = "cursor-pointer"; // not a Bootstrap class; provided as a common convenience if you define it
        }

        public static class Media
        {
            public const string ImgFluid = "img-fluid";
            public const string ImgThumbnail = "img-thumbnail";

            public static string Ratio(RatioKind ratio) => ratio switch
            {
                RatioKind.R1x1 => "ratio ratio-1x1",
                RatioKind.R4x3 => "ratio ratio-4x3",
                RatioKind.R16x9 => "ratio ratio-16x9",
                RatioKind.R21x9 => "ratio ratio-21x9",
                _ => throw new ArgumentOutOfRangeException(nameof(ratio))
            };

            public const string ObjectFitContain = "object-fit-contain";
            public const string ObjectFitCover = "object-fit-cover";
            public const string ObjectFitFill = "object-fit-fill";
            public const string ObjectFitScale = "object-fit-scale";
            public const string ObjectFitNone = "object-fit-none";
        }

        public static class Helpers
        {
            public const string StretchedLink = "stretched-link";
            public const string Vr = "vr";
            public const string Fade = "fade";
            // Stacks
            public const string HStack = "hstack";
            public const string VStack = "vstack";
        }

        public static class Btn
        {
            public const string Base = "btn";

            // Solid variants
            public const string Primary = "btn btn-primary";
            public const string Secondary = "btn btn-secondary";
            public const string Success = "btn btn-success";
            public const string Danger = "btn btn-danger";
            public const string Warning = "btn btn-warning";
            public const string Info = "btn btn-info";
            public const string Light = "btn btn-light";
            public const string Dark = "btn btn-dark";
            public const string Link = "btn btn-link";
            public const string Close = "btn btn-close";
            // Outline variants
            public const string OutlinePrimary = "btn btn-outline-primary";
            public const string OutlineSecondary = "btn btn-outline-secondary";
            public const string OutlineSuccess = "btn btn-outline-success";
            public const string OutlineDanger = "btn btn-outline-danger";
            public const string OutlineWarning = "btn btn-outline-warning";
            public const string OutlineInfo = "btn btn-outline-info";
            public const string OutlineLight = "btn btn-outline-light";
            public const string OutlineDark = "btn btn-outline-dark";

            // Sizes
            public const string Sm = "btn-sm";
            public const string Lg = "btn-lg";

            // States / layouts
            public const string Disabled = "disabled";
            public const string Active = "active";
            public const string Group = "btn-group";
            public const string GroupVertical = "btn-group-vertical";
            public const string Toolbar = "btn-toolbar";

            public static string Variant(Color c, bool outline = false)
            {
                if (!outline)
                {
                    return c switch
                    {
                        Color.Primary => Primary,
                        Color.Secondary => Secondary,
                        Color.Success => Success,
                        Color.Danger => Danger,
                        Color.Warning => Warning,
                        Color.Info => Info,
                        Color.Light => Light,
                        Color.Dark => Dark,
                        _ => throw new ArgumentOutOfRangeException(nameof(c))
                    };
                }

                return c switch
                {
                    Color.Primary => OutlinePrimary,
                    Color.Secondary => OutlineSecondary,
                    Color.Success => OutlineSuccess,
                    Color.Danger => OutlineDanger,
                    Color.Warning => OutlineWarning,
                    Color.Info => OutlineInfo,
                    Color.Light => OutlineLight,
                    Color.Dark => OutlineDark,
                    _ => throw new ArgumentOutOfRangeException(nameof(c))
                };
            }
        }

        public static class Alert
        {
            public const string Base = "alert";
            public const string Dismissible = "alert-dismissible";
            public const string Heading = "alert-heading";
            public const string Link = "alert-link";

            public static string Variant(Color c) => c switch
            {
                Color.Primary => "alert alert-primary",
                Color.Secondary => "alert alert-secondary",
                Color.Success => "alert alert-success",
                Color.Danger => "alert alert-danger",
                Color.Warning => "alert alert-warning",
                Color.Info => "alert alert-info",
                Color.Light => "alert alert-light",
                Color.Dark => "alert alert-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        public static class Badge
        {
            public const string Base = "badge";
            public const string Pill = "rounded-pill";

            public static string Variant(Color c) => c switch
            {
                Color.Primary => "badge text-bg-primary",
                Color.Secondary => "badge text-bg-secondary",
                Color.Success => "badge text-bg-success",
                Color.Danger => "badge text-bg-danger",
                Color.Warning => "badge text-bg-warning",
                Color.Info => "badge text-bg-info",
                Color.Light => "badge text-bg-light",
                Color.Dark => "badge text-bg-dark",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        public static class Card
        {
            public const string Base = "card";
            public const string Header = "card-header";
            public const string Body = "card-body";
            public const string Footer = "card-footer";
            public const string Title = "card-title";
            public const string Subtitle = "card-subtitle";
            public const string Text = "card-text";
            public const string Link = "card-link";
            public const string ImgTop = "card-img-top";
            public const string ImgBottom = "card-img-bottom";
            public const string ImgOverlay = "card-img-overlay";
            public const string Group = "card-group";
        }

        public static class ListGroup
        {
            public const string Base = "list-group";
            public const string Item = "list-group-item";
            public const string Flush = "list-group-flush";
            public const string Numbered = "list-group-numbered";
            public const string Horizontal = "list-group-horizontal";
            public const string ItemAction = "list-group-item-action";

            public static string HorizontalAt(Breakpoint bp)
            {
                var b = Bp(bp);
                return $"list-group-horizontal-{b}";
            }
        }

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
                var b = Bp(bp);
                return $"table-responsive-{b}";
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
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        public static class Form
        {
            public const string Label = "form-label";
            public const string Text = "form-text";
            public const string Control = "form-control";
            public const string ControlSm = "form-control form-control-sm";
            public const string ControlLg = "form-control form-control-lg";
            public const string ControlPlaintext = "form-control-plaintext";
            public const string Select = "form-select";
            public const string SelectSm = "form-select form-select-sm";
            public const string SelectLg = "form-select form-select-lg";
            public const string Check = "form-check";
            public const string CheckInput = "form-check-input";
            public const string CheckLabel = "form-check-label";
            public const string Switch = "form-check form-switch";
            public const string Range = "form-range";
            public const string Floating = "form-floating";

            public const string InputGroup = "input-group";
            public const string InputGroupText = "input-group-text";
            public const string InputGroupSm = "input-group input-group-sm";
            public const string InputGroupLg = "input-group input-group-lg";

            public const string ValidFeedback = "valid-feedback";
            public const string InvalidFeedback = "invalid-feedback";
            public const string ValidTooltip = "valid-tooltip";
            public const string InvalidTooltip = "invalid-tooltip";

            public const string WasValidated = "was-validated";
            public const string IsValid = "is-valid";
            public const string IsInvalid = "is-invalid";
        }
        public static class Nav
        {
            public const string Base = "nav";
            public const string Item = "nav-item";
            public const string Link = "nav-link";
            public const string Tabs = "nav nav-tabs";
            public const string Pills = "nav nav-pills";
            public const string Fill = "nav-fill";
            public const string Justified = "nav-justified";
        }

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

            public const string Light = "navbar-light"; // if using legacy scheme (BS 5.3 prefers data-bs-theme)
            public const string Dark = "navbar-dark";
        }

        public static class Dropdown
        {
            public const string Base = "dropdown";
            public const string Toggle = "dropdown-toggle";
            public const string Menu = "dropdown-menu";
            public const string Item = "dropdown-item";
            public const string Divider = "dropdown-divider";
            public const string Header = "dropdown-header";

            public const string MenuEnd = "dropdown-menu-end";
            public const string MenuStart = "dropdown-menu-start";

            public static string MenuAlign(Placement p) => p switch
            {
                Placement.Start => "dropdown-menu-start",
                Placement.End => "dropdown-menu-end",
                _ => throw new ArgumentOutOfRangeException(nameof(p))
            };
        }

        public static class Modal
        {
            public const string ModalBase = "modal";
            public const string Dialog = "modal-dialog";
            public const string Content = "modal-content";
            public const string Header = "modal-header";
            public const string Body = "modal-body";
            public const string Footer = "modal-footer";
            public const string Title = "modal-title";
            public const string Backdrop = "modal-backdrop";

            public const string DialogCentered = "modal-dialog-centered";
            public const string DialogScrollable = "modal-dialog-scrollable";

            public const string Sm = "modal-sm";
            public const string Lg = "modal-lg";
            public const string Xl = "modal-xl";
            public const string Fullscreen = "modal-fullscreen";

            public static string FullscreenAt(Breakpoint bp) => $"modal-fullscreen-{Bp(bp)}-down";
        }

        public static class Offcanvas
        {
            public const string Base = "offcanvas";
            public const string Header = "offcanvas-header";
            public const string Body = "offcanvas-body";
            public const string Title = "offcanvas-title";

            public const string Start = "offcanvas-start";
            public const string End = "offcanvas-end";
            public const string Top = "offcanvas-top";
            public const string Bottom = "offcanvas-bottom";
        }

        public static class Toast
        {
            public const string Container = "toast-container";
            public const string ToastBase = "toast";
            public const string Header = "toast-header";
            public const string Body = "toast-body";
        }

        public static class Accordion
        {
            public const string Base = "accordion";
            public const string Item = "accordion-item";
            public const string Header = "accordion-header";
            public const string Button = "accordion-button";
            public const string Collapse = "accordion-collapse";
            public const string Body = "accordion-body";
            public const string Flush = "accordion-flush";

            public const string ButtonCollapsed = "collapsed";
        }

        public static class Breadcrumb
        {
            public const string Base = "breadcrumb";
            public const string Item = "breadcrumb-item";
        }

        public static class Pagination
        {
            public const string Base = "pagination";
            public const string Sm = "pagination pagination-sm";
            public const string Lg = "pagination pagination-lg";
            public const string Item = "page-item";
            public const string Link = "page-link";

            public const string Active = "active";
            public const string Disabled = "disabled";
        }

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
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

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
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        public static class Common
        {
            // Centering helpers
            public const string CenterContent = "d-flex justify-content-center align-items-center";
            public const string CenterText = "text-center";
            public const string CenterBlock = "mx-auto";

            // Sticky top
            public const string StickyTop = "sticky-top";
            public const string StickyBottom = "sticky-bottom";
        }

        public static class Misc
        {
            // Width/height helpers (often used in components)
            public const string W100 = "w-100";
            public const string H100 = "h-100";

            // Borders/shapes
            public const string Rounded = "rounded";
            public const string RoundedPill = "rounded-pill";
            public const string RoundedCircle = "rounded-circle";

            // Cursor-ish (Bootstrap doesn't ship cursor utilities; include your own if desired)
            public const string CursorPointer = "cursor-pointer";
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
