using HeimdallTemplateApp.Models;

namespace HeimdallTemplateApp.Services;

public sealed class WeatherForecastStore
{
    private readonly IReadOnlyList<WeatherRow> _rows = BuildRows();

    public int Count => _rows.Count;

    public IReadOnlyList<WeatherRow> GetRows(int offset, int take)
    {
        if (offset < 0)
            offset = 0;

        if (take <= 0)
            take = 10;

        return _rows.Skip(offset).Take(take).ToArray();
    }

    private static IReadOnlyList<WeatherRow> BuildRows()
    {
        var start = DateTime.UtcNow.Date.AddDays(-99);
        var list = new List<WeatherRow>(capacity: 1000);

        for (var i = 0; i < 1000; i++)
        {
            var utc = start.AddDays(i).AddHours(12);
            var temp = 5m + (decimal)(12.0 * Math.Sin(i / 7.0)) + (i % 5);
            var humidity = 35 + (int)(40.0 * (0.5 + 0.5 * Math.Cos(i / 9.0)));
            var wind = 3m + (decimal)(12.0 * (0.5 + 0.5 * Math.Sin(i / 5.0)));

            var condition = (i % 6) switch
            {
                0 => "Sunny",
                1 => "Partly Cloudy",
                2 => "Overcast",
                3 => "Rain",
                4 => "Windy",
                _ => "Fog"
            };

            list.Add(new WeatherRow(
                utc,
                $"Station {(i % 4) + 1}",
                Math.Round(temp, 1),
                Math.Clamp(humidity, 10, 95),
                Math.Round(wind, 1),
                condition));
        }

        return list;
    }
}
