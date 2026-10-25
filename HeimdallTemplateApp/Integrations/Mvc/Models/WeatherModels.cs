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

public sealed class WeatherRowsViewModel
{
	public IReadOnlyList<WeatherRow> Rows { get; init; } = [];
	public int Offset { get; init; }
	public int Take { get; init; } = 10;
	public int TotalCount { get; init; }
}
