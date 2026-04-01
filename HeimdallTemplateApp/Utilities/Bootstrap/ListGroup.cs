
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
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
                if (bp == Breakpoint.None)
                    return Horizontal;

                return $"list-group-horizontal-{Bp(bp)}";
            }
        }
    }
}
