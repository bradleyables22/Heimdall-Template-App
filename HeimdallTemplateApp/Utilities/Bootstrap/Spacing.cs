
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
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

            // gap
            public static string Gap(int n, Breakpoint bp = Breakpoint.None) => GapInternal("gap", n, bp);
            public static string GapX(int n, Breakpoint bp = Breakpoint.None) => GapInternal("column-gap", n, bp);
            public static string GapY(int n, Breakpoint bp = Breakpoint.None) => GapInternal("row-gap", n, bp);

            private static string GapInternal(string prefix, int n, Breakpoint bp)
            {
                if (n < 0 || n > 5)
                    throw new ArgumentOutOfRangeException(nameof(n), "Bootstrap scale is 0..5.");

                return bp == Breakpoint.None
                    ? $"{prefix}-{n}"
                    : $"{prefix}-{Bp(bp)}-{n}";
            }

            private static string Space(string prefix, int n, Side side, Breakpoint bp)
            {
                if (n < 0 || n > 5)
                    throw new ArgumentOutOfRangeException(nameof(n), "Bootstrap spacing scale is 0..5.");

                var s = SideToken(side);

                if (bp == Breakpoint.None)
                    return side == Side.None ? $"{prefix}-{n}" : $"{prefix}{s}-{n}";

                return side == Side.None
                    ? $"{prefix}-{Bp(bp)}-{n}"
                    : $"{prefix}{s}-{Bp(bp)}-{n}";
            }

            private static string SpaceAuto(string prefix, Side side, Breakpoint bp)
            {
                var s = SideToken(side);

                if (bp == Breakpoint.None)
                    return side == Side.None ? $"{prefix}-auto" : $"{prefix}{s}-auto";

                return side == Side.None
                    ? $"{prefix}-{Bp(bp)}-auto"
                    : $"{prefix}{s}-{Bp(bp)}-auto";
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
    }
}
