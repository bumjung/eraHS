using eraHS.Constants.Hearthstone;
using eraHS.Utility.RegexHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace eraHS.LogReader
{
    class LogManager
    {
        private LogReader _logReader;
        private Dictionary<int, string> _heroEntityDict;
        private Dictionary<string, int> _playerEntityDict;

        public static Barrier sync;
        public static Semaphore sem;

        public List<String> gameLogLines;
        public List<String> copyGameLogLines;

        public LogManager()
        {
            gameLogLines = new List<String>();
            copyGameLogLines = new List<String>();
            _logReader = new LogReader(gameLogLines);
            _heroEntityDict = new Dictionary<int, string>();
            _playerEntityDict = new Dictionary<string, int>();
        }

        public void start()
        {
            sync = new Barrier(participantCount: 2);
            sem = new Semaphore(1, 1);
            while (true)
            {
                var readingLogFile = new Thread(() =>
                {
                    _logReader.readLogFile();
                });
                readingLogFile.Start();


                var parsingLogLines = new Thread(() =>
                {
                    this.parseLogLines();
                });
                parsingLogLines.Start();

                readingLogFile.Join();
                parsingLogLines.Join();

                foreach (string line in gameLogLines)
                {
                    copyGameLogLines.Add(line);
                }
                gameLogLines.Clear();

            }
        }

        private void parseLogLines()
        {
            int count = 0;
            foreach (string line in copyGameLogLines)
            {
                Match heroMatch = RegexManager.heroIdRegex.Match(line);
                if (heroMatch.Success)
                {
                    int entityId = Int32.Parse(heroMatch.Groups[1].Value);
                    int heroId = Int32.Parse(heroMatch.Groups[2].Value) - 1;
                    _heroEntityDict.Add(entityId, Hero.HeroIdList[heroId]);
                }

                Match playerMatch = RegexManager.playerNameRegex.Match(line);
                if (playerMatch.Success)
                {
                    string username = playerMatch.Groups[1].Value;
                    int entityId = Int32.Parse(playerMatch.Groups[2].Value);
                    _playerEntityDict.Add(simplifyString(username), entityId);
                }

                Match gameResultMatch = RegexManager.gameResultRegex.Match(line);
                if (gameResultMatch.Success)
                {
                    count++;
                    string username = gameResultMatch.Groups[1].Value;
                    string result = gameResultMatch.Groups[2].Value;
                    if (_playerEntityDict.ContainsKey(simplifyString(username)))
                    {
                        Console.WriteLine(username + '\t'
                            + _heroEntityDict[_playerEntityDict[simplifyString(username)]]
                            + '\t' + result);
                    }

                    if (count % 2 == 0)
                    {
                        _playerEntityDict.Clear();
                        _heroEntityDict.Clear();
                    }
                }
            }
            copyGameLogLines.Clear();
            sync.SignalAndWait();
        }

        private string simplifyString(string str)
        {
            return str.Replace(' ', '_').ToLower();
        }
    }
}
