
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
        public static class Position
        {
            public const string Static = "position-static";
            public const string Relative = "position-relative";
            public const string Absolute = "position-absolute";
            public const string Fixed = "position-fixed";
            public const string Sticky = "position-sticky";
            public const string StickyTop = "sticky-top";
            public const string StickyBottom = "sticky-bottom";

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
    }
}
