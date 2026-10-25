namespace HeimdallTemplateApp.Models;

public sealed record WeatherRow(
    DateTime Utc,
    string Station,
    decimal TempC,
    int HumidityPct,
    decimal WindMph,
    string Condition);

public sealed class WeatherLoadMoreRequest
{
    public int Offset { get; set; }
    public int Take { get; set; } = 10;
}
