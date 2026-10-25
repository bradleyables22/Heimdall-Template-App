namespace HeimdallTemplateApp.Models;

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

public sealed class ToastItem
{
    public string Header { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public ToastType Type { get; init; } = ToastType.Info;
    public int DurationMs { get; init; } = 3000;

    public string BootstrapClass => Type switch
    {
        ToastType.Success => "text-bg-success",
        ToastType.Error => "text-bg-danger",
        ToastType.Warning => "text-bg-warning",
        ToastType.Info => "text-bg-info",
        _ => "text-bg-secondary"
    };
}
