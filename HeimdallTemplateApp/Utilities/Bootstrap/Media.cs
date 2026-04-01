namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
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
    }
}
