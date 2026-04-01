
namespace HeimdallTemplateApp.Utilities
{
    public static partial class Bootstrap
    {
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

            public static string FullscreenAt(Breakpoint bp)
            {
                if (bp == Breakpoint.None)
                    return Fullscreen;

                return $"modal-fullscreen-{Bp(bp)}-down";
            }
        }
    }
}
