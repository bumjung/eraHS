using eraHS.Constants.Hearthstone;
using eraHS.Utility.RegexHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace eraHS.LogReader
{
    class LogReader
    {
        string logFilePath;

        public LogReader()
        {
            this.init();
        }

        void init()
        {
            logFilePath = Config.userFilePath + @"\Power.log";
        }

        public void readLogFile()
        {
            Dictionary<int, String> heroEntityDict = new Dictionary<int, String>();
            Dictionary<String, int> playerEntityDict = new Dictionary<String, int>();
            string[] lines = System.IO.File.ReadAllLines(logFilePath);
            int count = 0;

            foreach (string line in lines)
            {
                Match heroMatch = RegexManager.heroIdRegex.Match(line);
                if (heroMatch.Success)
                {
                    int heroId = Int32.Parse(heroMatch.Groups[2].Value) - 1;
                    // Console.WriteLine(Hero.HeroIdList[heroId] + " " + heroMatch.Groups[1].Value);
                    heroEntityDict.Add(Int32.Parse(heroMatch.Groups[1].Value), Hero.HeroIdList[heroId]);

                }
                Match playerMatch = RegexManager.playerNameRegex.Match(line);
                if (playerMatch.Success)
                {
                    // Console.WriteLine(playerMatch.Groups[1].Value + " " + playerMatch.Groups[2].Value);
                    playerEntityDict.Add(simplifyString(playerMatch.Groups[1].Value), Int32.Parse(playerMatch.Groups[2].Value));
                }
                Match gameResultMatch = RegexManager.gameResultRegex.Match(line);
                if (gameResultMatch.Success)
                {
                    count++;
                    // Console.WriteLine(gameResultMatch.Groups[1].Value + " " + gameResultMatch.Groups[2].Value);
                    string name = simplifyString(gameResultMatch.Groups[1].Value);
                    if (playerEntityDict.ContainsKey(name)) {
                        Console.WriteLine(gameResultMatch.Groups[1].Value + '\t' + heroEntityDict[playerEntityDict[name]] + '\t' + gameResultMatch.Groups[2].Value);
                    }

                    if (count % 2 == 0)
                    {

                        playerEntityDict.Clear();
                        heroEntityDict.Clear();
                    }
                }
            }
        }

        private string simplifyString(string str)
        {
            return str.Replace(' ', '_').ToLower();
        }

    }
}
