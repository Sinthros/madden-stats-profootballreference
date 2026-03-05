using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaddenImporter.Models.Player;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using AngleSharp;

namespace MaddenImporter.Core
{
    public class SeasonalRetriever : IDisposable
    {
        private AngleSharp.IBrowsingContext browser;
        private ChromeDriver driver;
        private static readonly Random _random = new Random();

        public SeasonalRetriever(IBrowsingContext br = null)
        {
            browser = br ?? Extensions.GetDefaultBrowser();

            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36");

            // Persist session so Cloudflare trust sticks
            options.AddArgument(@"--user-data-dir=C:\selenium-profile");

            driver = new ChromeDriver(options);
        }

        private static string GetSeasonUrl(int year, PlayerType playerType) =>
            $"https://www.pro-football-reference.com/years/{year}/{playerType.ToString().ToLower()}.htm";

        private async Task<IEnumerable<string>> GetPlayersJson(int year, PlayerType playerType)
        {
            var url = GetSeasonUrl(year, playerType);

            Console.WriteLine($"Now retrieving {playerType} players.");
            Console.WriteLine(url);

            driver.Navigate().GoToUrl(url);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(d => d.FindElements(By.CssSelector("table.stats_table")).Count > 0);

            var pageSource = driver.PageSource;

            var context = Extensions.GetDefaultBrowser();
            var document = await context.OpenAsync(req => req.Content(pageSource));

            // ignore partial_table rows
            var playerRows = document
                .QuerySelectorAll("table[data-soc-sum-phase-type='reg'] > tbody > tr")
                .Where(tr =>
                    !tr.ClassList.Contains("thead") &&
                    !tr.ClassList.Contains("partial_table")
                )
                .Select(el => el.Children);

            List<string> jsons = new List<string>();

            foreach (var row in playerRows)
            {
                var json = "{";

                foreach (var td in row)
                {
                    var name = td.GetAttribute("data-stat")?.ToLower();
                    if (string.IsNullOrEmpty(name)) continue;

                    dynamic value;

                    var anchor = td.QuerySelector("a");
                    if (anchor != null && (name == "name_display" || name == "player"))
                    {
                        var href = anchor.GetAttribute("href");
                        if (!string.IsNullOrEmpty(href))
                        {
                            if (href.StartsWith("/"))
                                href = "https://www.pro-football-reference.com" + href;

                            json += $"\"PlayerLink\": \"{href}\",";
                        }
                    }

                    var text = td.TextContent?.Trim();

                    if (int.TryParse(text, out int intVal))
                        value = intVal;
                    else if (float.TryParse(text, out float floatVal))
                        value = floatVal;
                    else if (!string.IsNullOrEmpty(text))
                        value = $"\"{text.Replace("\"", "\\\"")}\"";
                    else
                    {
                        if (name == "pos") value = "\"N/A\"";
                        else value = 0;
                    }

                    json += $"\"{name}\": {value},";
                }

                if (json.EndsWith(","))
                    json = json.Substring(0, json.Length - 1);

                json += "}";
                jsons.Add(json);
            }

            // human-like delay between page loads
            await Task.Delay(_random.Next(2500, 4500));

            return jsons;
        }

        public async Task<IEnumerable<Player>> GetAllPlayers(int year)
        {
            IEnumerable<Player> players = new List<Player>();

            var types = new PlayerType[]
            {
                PlayerType.Defense,
                PlayerType.Passing,
                PlayerType.Receiving,
                PlayerType.Rushing,
                PlayerType.Returns,
                PlayerType.Kicking,
                PlayerType.Punting
            };

            foreach (var enumType in types)
            {
                var retrieved = await GetPlayersJson(year, enumType);

                Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.\n");

                players = players.Concat(
                    retrieved.Select(p => enumType.ConvertFromJson(p, Extensions.RemapKeys))
                );
            }

            return players;
        }

        public void Dispose()
        {
            driver?.Quit();
            driver?.Dispose();
            browser?.Dispose();
        }
    }
}