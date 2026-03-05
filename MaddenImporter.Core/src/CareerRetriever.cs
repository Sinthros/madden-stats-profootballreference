using System;
using System.Collections.Generic;
using System.Linq;
using MaddenImporter.Models.Player;
using OpenQA.Selenium;
using AngleSharp;

namespace MaddenImporter.Core
{
    public class CareerRetriever : IDisposable
    
    {
        private IWebDriver driver;
        private const string loginUrl = "https://stathead.com/users/login.cgi";

        private static readonly Dictionary<PlayerType, string> urlSuffix = new Dictionary<PlayerType, string>
        {
            { PlayerType.Defense, "&order_by=def_int&positions[]=dt&positions[]=de&positions[]=dl&positions[]=ilb&positions[]=olb&positions[]=lb&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=sacks&ccomp[1]=gt&cval[1]=0&cstat[2]=fumbles_rec&ccomp[2]=gt&cval[2]=0&cstat[3]=tackles_solo&ccomp[3]=gt&cval[3]=0&cstat[4]=safety_md&ccomp[4]=gt&cval[4]=0" },//"https://stathead.com/football/player-season-finder.cgi?request=1&match=player_season_combined&order_by_asc=0&order_by=def_int&year_min=1995&year_max=2023&positions%5B%5D=dt&positions%5B%5D=de&positions%5B%5D=dl&positions%5B%5D=ilb&positions%5B%5D=olb&positions%5B%5D=lb&positions%5B%5D=cb&positions%5B%5D=s&positions%5B%5D=db&comp_type=reg&ccomp%5B1%5D=gt&cval%5B1%5D=0&cstat%5B1%5D=sacks&ccomp%5B2%5D=gt&cval%5B2%5D=0&cstat%5B2%5D=fumbles_rec&ccomp%5B3%5D=gt&cval%5B3%5D=0&cstat%5B3%5D=tackles_solo&ccomp%5B4%5D=gt&cval%5B4%5D=0&cstat%5B4%5D=safety_md&season_start=1&season_end=-1&weight_min=0&weight_max=500&draft_year_min=1936&draft_year_max=2023&draft_slot_min=1&draft_slot_max=500&draft_pick_in_round=pick_overall&conference=any&seasons_played_comp=gt&is_active=Y&pro_bowls_comp=gt&all_pros_first_team_comp=gt"},//"&order_by=def_int&positions[]=dt&positions[]=de&positions[]=dl&positions[]=ilb&positions[]=olb&positions[]=lb&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=sacks&ccomp[1]=gt&cval[1]=0&cstat[2]=fumbles_rec&ccomp[2]=gt&cval[2]=0&cstat[3]=tackles_solo&ccomp[3]=gt&cval[3]=0&cstat[4]=safety_md&ccomp[4]=gt&cval[4]=0" },
                       
            { PlayerType.Kicking, "&order_by=punt&positions[]=k&positions[]=p&cstat[1]=xpm&ccomp[1]=gt&cval[1]=0" },
            { PlayerType.Passing,  "&order_by=pass_cmp&positions[]=qb" },
            { PlayerType.Receiving, "&order_by=rec&positions[]=rb&positions[]=wr&positions[]=te" },
            { PlayerType.Returns, "&order_by=kick_ret&positions[]=rb&positions[]=wr&positions[]=cb&positions[]=s&positions[]=db&cstat[1]=punt_ret&ccomp[1]=gt&cval[1]=0" },
            { PlayerType.Rushing, "&order_by=rush_att&positions[]=qb&positions[]=rb&positions[]=wr&positions[]=te&cstat[1]=fumbles&ccomp[1]=gt&cval[1]=0" }
        };

        public CareerRetriever(string geckoPath, IBrowsingContext br = null)
        {
            var firefoxOptions = new OpenQA.Selenium.Firefox.FirefoxOptions();
            firefoxOptions.AddArgument("-headless");
            driver = new OpenQA.Selenium.Firefox.FirefoxDriver(geckoPath, firefoxOptions);
        }

        private static string GetCareerUrl(PlayerType playerType, int offset)
        {
            urlSuffix.TryGetValue(playerType, out string suffix);
            suffix ??= "";
            //Console.WriteLine(suffix);
            //     Base URL  ->                                                       Combined seasons                                                          Start year                                       End year                                         Check for >= 1 games played                      Draft status           Offset     
            return $"https://stathead.com/football/player-season-finder.cgi?request=1&match=player_season_combined&season_start=1&order_by_asc=0&conference=any&year_min=1990&draft_slot_max=500&match=combined=&year_max=2015&season_end=-1&age_min=0&age_max=99&ccomp%5B2%5D=gt&cval%5B2%5D=1&cstat%5B2%5D=games&draft_status=undrafted&offset={offset}{suffix}";

            //return $"https://stathead.com/football/player-season-finder.cgi?request=1&match=player_season_combined&season_start=1&order_by_asc=0&conference=any&year_min=1995&draft_slot_max=500&match=combined=&year_max={DateTime.Now.Year}&season_end=-1&is_active=Y&age_min=0&age_max=99&offset={offset}{suffix}";
        }

        private IEnumerable<string> GetPageAsJson(IEnumerable<IEnumerable<AngleSharp.Dom.IElement>> playerRows, string pos)
        {
            List<string> jsons = new List<string>();
            int playerRowCount = playerRows.Count();
            foreach (var row in playerRows)
            {
                int count = row.Count() - 1;
                var json = "{";
                json += $"\"pos\": \"{pos}\",";
                foreach (var td in row)
                {
                    var name = td.GetAttribute("data-stat").ToLower();
                    dynamic value;
                                        // Check if the cell contains an <a> tag
                    var anchor = td.QuerySelector("a");
                    if (anchor != null && (name == "name_display" || name == "player")) // Adjust if the href is tied to a different key
                    {
                        var href = anchor.GetAttribute("href");
                        if (!string.IsNullOrEmpty(href))
                        {
                            // Remove query parameters if present
                            if (href.Contains("?"))
                            {
                                href = href.Split('?')[0];
                            }

                            // Prepend base URL if href is relative
                            if (href.StartsWith("/"))
                            {
                                href = "https://www.pro-football-reference.com" + href;
                            }
                            json += $"\"PlayerLink\": \"{href}\",";
                        }
                    }
                    var intOk = int.TryParse(td.TextContent, out int @int);
                    var floatOk = float.TryParse(td.TextContent, out float @float);
                    var str = td.TextContent?.Trim();
                    if (intOk)
                        value = @int;
                    else if (floatOk)
                        value = @float;
                    else if (!string.IsNullOrEmpty(str))
                        value = $"\"{str}\"";
                    else
                    {
                        if (name == "pos") value = "\"N/A\"";
                        else value = 0;
                    }

                    json += $"\"{name}\": {value.ToString()},";
                }
                json = json.Substring(0, json.Length - 1); // remove trailing comma
                json += "}";
                jsons.Add(json);
                //Console.Write($"\rParsing... {--playerRowCount} remaining.");
            }
            return jsons;
        }

        private IEnumerable<string> GetPlayersJson(PlayerType playerType)
        {
            // pull data
            int offset = 0;
            Extensions.PlayerPositions.TryGetValue(playerType, out string pos);
            var url = GetCareerUrl(playerType, offset);
            driver.Navigate().GoToUrl(url);
            Console.WriteLine(url);
            Console.WriteLine($"Now retrieving {playerType} players.");
            int playerRowCount = 0;
            List<string> jsons = new List<string>();
            bool paginate = true;
            var parser = new AngleSharp.Html.Parser.HtmlParser();

            do
            {
                var html = driver.PageSource;
                var document = parser.ParseDocument(html);
                var playerRows = document.QuerySelectorAll("tbody > tr:not(.thead)")
                .Select(el => el.Children.ToList());

                var page = GetPageAsJson(playerRows, pos);
                jsons = jsons.Concat(page).ToList();

                playerRowCount = playerRows.Count();
                paginate = playerRowCount == 200;
                if (paginate)
                {
                    offset += 200;
                    Console.WriteLine("Going to next page...");
                    url = GetCareerUrl(playerType, offset);

                    driver.Navigate().GoToUrl(url);
                }
            } while (paginate);

            return jsons;
        }

        public IEnumerable<Player> GetAllPlayers(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            IEnumerable<Player> players = new List<Player>();
            var types = new PlayerType[] { PlayerType.Defense, PlayerType.Passing, PlayerType.Receiving,
            PlayerType.Rushing, PlayerType.Returns, PlayerType.Kicking };

            // login
            driver.Navigate().GoToUrl(loginUrl);
            driver.FindElement(By.Id("username")).SendKeys(username);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.CssSelector("input[type=submit]")).Click();

            try
            {
                var error = driver.FindElement(By.CssSelector("div.error"));
                if (error != null && error.Displayed)
                    throw new ApplicationException();
            }
            // login OK
            catch (NoSuchElementException) { }

            foreach (var enumType in types)
            {
                var retrieved = GetPlayersJson(enumType);
                Console.WriteLine("wtf");
                var r = retrieved.Select(p => enumType.ConvertFromJson(p, Extensions.RemapKeys));
                Console.WriteLine($"Retrieved {retrieved.Count()} {enumType} players.");
                players = players.Concat(r);
            }

            return players;
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
