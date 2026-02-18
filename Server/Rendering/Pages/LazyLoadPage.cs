using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Utilities;

namespace Server.Rendering.Pages
{
    public static class LazyLoadPage
    {
        private sealed record WeatherRow(
            DateTime Utc,
            string Station,
            decimal TempC,
            int HumidityPct,
            decimal WindMph,
            string Condition);

        private static readonly List<WeatherRow> _rows = BuildRows();

        private static List<WeatherRow> BuildRows()
        {
            var start = DateTime.UtcNow.Date.AddDays(-99);
            var list = new List<WeatherRow>(capacity: 1000);

            for (int i = 0; i < 1000; i++)
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
                    Utc: utc,
                    Station: $"Station {(i % 4) + 1}",
                    TempC: Math.Round(temp, 1),
                    HumidityPct: Math.Clamp(humidity, 10, 95),
                    WindMph: Math.Round(wind, 1),
                    Condition: condition));
            }

            return list;
        }

        public sealed class LoadMoreRequest
        {
            public int Offset { get; set; }
            public int Take { get; set; } = 10;
        }

        public static IHtmlContent Render()
        {
            const int initialTake = 10;
            var first = _rows.Take(initialTake).ToList();

            return FluentHtml.Div(page =>
            {
                page.Class(Bootstrap.Layout.Container, Bootstrap.Spacing.Mt(4));

                page.Div(card =>
                {
                    card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg);

                    card.Div(header =>
                    {
                        header.Class(Bootstrap.Card.Header);
                        header.H2(h => h.Text("Lazy-loading Scroll Table (Weather Data)"));
                    });

                    card.Div(body =>
                    {
                        body.Class(Bootstrap.Card.Body);

                        body.P(p =>
                        {
                            p.Class(Bootstrap.Text.BodySecondary);
                            p.Text("Scroll the table. When the sentinel row becomes visible, it loads the next page.");
                        });

                        body.Div(scroller =>
                        {
                            scroller.Class(Bootstrap.Border.Default, Bootstrap.Border.Rounded, Bootstrap.Spacing.P(2));
                            scroller.Style("overflow: auto;");

                            scroller.Tag("table", table =>
                            {
                                table.Class(
                                    Bootstrap.Table.Base,
                                    Bootstrap.Table.Hover,
                                    Bootstrap.Table.Sm,
                                    Bootstrap.Spacing.Mb(0));

                                table.Tag("thead", thead =>
                                {
                                    thead.Tag("tr", tr =>
                                    {
                                        tr.Tag("th", th => th.Text("UTC"));
                                        tr.Tag("th", th => th.Text("Station"));
                                        tr.Tag("th", th => th.Text("Temp (°C)"));
                                        tr.Tag("th", th => th.Text("Humidity (%)"));
                                        tr.Tag("th", th => th.Text("Wind (mph)"));
                                        tr.Tag("th", th => th.Text("Condition"));
                                    });
                                });

                                table.Tag("tbody", tbody =>
                                {
                                    tbody.Id("weather-rows");
                                    tbody.Add(RenderWeatherRows(first));
                                    tbody.Add(RenderSentinelRow(offset: initialTake, take: 10));
                                });
                            });
                        });
                    });
                });
            });
        }

        private static IHtmlContent RenderWeatherRows(IEnumerable<WeatherRow> rows)
        {
            return FluentHtml.Fragment(f =>
            {
                foreach (var r in rows)
                    f.Add(RenderWeatherRow(r));
            });
        }

        private static IHtmlContent RenderSentinelRow(int offset, int take)
        {
            var hasMore = offset < _rows.Count;

            return FluentHtml.Tag("tr", tr =>
            {
                tr.Id("weather-sentinel");

                if (hasMore)
                {
                    // State lives on the <tr>. The <td> trigger walks up one level
                    // and finds it immediately via findClosestStateElement.
                    tr.Heimdall().State("weather", new { offset, take });
                }

                tr.Tag("td", td =>
                {
                    td.Attr("colspan", "6");
                    td.Class(Bootstrap.Text.Center, Bootstrap.Text.BodySecondary, Bootstrap.Spacing.Py(2));

                    if (!hasMore)
                    {
                        td.Text("✅ All records loaded.");
                        return;
                    }

                    // Trigger on <td>, state on parent <tr>, target the <tr> for swap.
                    td.Heimdall()
                        .Visible(new HeimdallHtml.ActionId("LazyLoadPage.LoadMore"))
                        .PayloadFromClosestState("weather")
                        .Target("#weather-sentinel")
                        .SwapOuter()
                        .VisibleOnce(true);

                    td.Text("Loading more…");
                });
            });
        }


        [ContentInvocation]
        public static IHtmlContent LoadMore(LoadMoreRequest req)
        {
            req ??= new LoadMoreRequest();
            if (req.Take <= 0) req.Take = 10;
            if (req.Offset < 0) req.Offset = 0;

            var next = _rows
                .Skip(req.Offset)
                .Take(req.Take)
                .ToList();

            var nextOffset = req.Offset + next.Count;

            // inserting the new rows and a fresh sentinel in one operation.
            return FluentHtml.Fragment(f =>
            {
                foreach (var row in next)
                    f.Add(RenderWeatherRow(row));

                f.Add(RenderSentinelRow(offset: nextOffset, take: req.Take));
            });
        }

        private static IHtmlContent RenderWeatherRow(WeatherRow r)
        {
            return FluentHtml.Tag("tr", tr =>
            {
                tr.Tag("td", td => td.Text(r.Utc.ToString("yyyy-MM-dd HH:mm")));
                tr.Tag("td", td => td.Text(r.Station));
                tr.Tag("td", td => td.Text(r.TempC.ToString("0.0")));
                tr.Tag("td", td => td.Text(r.HumidityPct.ToString()));
                tr.Tag("td", td => td.Text(r.WindMph.ToString("0.0")));
                tr.Tag("td", td => td.Text(r.Condition));
            });
        }
    }
}