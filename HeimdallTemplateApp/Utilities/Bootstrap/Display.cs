
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
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

            public static string Kind(DisplayKind kind) => kind switch
            {
                DisplayKind.Block => Block,
                DisplayKind.Inline => Inline,
                DisplayKind.InlineBlock => InlineBlock,
                DisplayKind.Grid => Grid,
                DisplayKind.InlineGrid => InlineGrid,
                DisplayKind.Flex => Flex,
                DisplayKind.InlineFlex => InlineFlex,
                DisplayKind.Table => Table,
                DisplayKind.TableRow => TableRow,
                DisplayKind.TableCell => TableCell,
                DisplayKind.None => None,
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            public static string At(Breakpoint bp, DisplayKind kind)
            {
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
                    DisplayKind.None => "none",
                    _ => throw new ArgumentOutOfRangeException(nameof(kind))
                };

                return bp == Breakpoint.None
                    ? $"d-{k}"
                    : $"d-{Bp(bp)}-{k}";
            }

            public const string Display1 = "display-1";
            public const string Display2 = "display-2";
            public const string Display3 = "display-3";
            public const string Display4 = "display-4";
            public const string Display5 = "display-5";
            public const string Display6 = "display-6";

            public static string DisplaySize(DisplaySizeKind size) => size switch
            {
                DisplaySizeKind.Display1 => Display1,
                DisplaySizeKind.Display2 => Display2,
                DisplaySizeKind.Display3 => Display3,
                DisplaySizeKind.Display4 => Display4,
                DisplaySizeKind.Display5 => Display5,
                DisplaySizeKind.Display6 => Display6,
                _ => throw new ArgumentOutOfRangeException(nameof(size))
            };
        }
    }
}
