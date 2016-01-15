using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using eraHS.Constants.Hearthstone;
using eraHS.Utility;
using eraHS.Utility.RegexHelper;

namespace eraHS.LogReader
{
    class LogManager
    {
        private LogReader _logReader;
        private LogWatcher _logWatcher;
        private Dictionary<int, string> _heroEntityDict;
        private Dictionary<string, int> _playerEntityDict;
        private Dictionary<int, string> _playerIdDict;

        public static Barrier barrier;
        public static BinarySemaphore sem;

        public List<String> gameLogLines;
        public List<String> copyGameLogLines;

        public LogManager()
        {
            gameLogLines = new List<String>();
            copyGameLogLines = new List<String>();

            _logReader = new LogReader(gameLogLines);
            _logWatcher = new LogWatcher(@"Power*.log");

            _heroEntityDict = new Dictionary<int, string>();
            _playerEntityDict = new Dictionary<string, int>();
            _playerIdDict = new Dictionary<int, string>();

            barrier = new Barrier(participantCount: 2);
            sem = new BinarySemaphore(0, 1);

        }

        public void start()
        {
            while (true)
            {
                var readingLogFile = new Thread(() =>
                {
                    _logWatcher.start();
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

                copyGameLogLines.ShallowCopy(gameLogLines);

                gameLogLines.Clear();
            }
        }

        private void parseLogLines()
        {
            int count = 0;
            int myId = -1;
            Json playerJson = new Json();
            Json opponentJson = new Json();
            Json resultJson = new Json();

            foreach (string line in copyGameLogLines)
            {
                Match heroMatch = RegexManager.heroIdRegex.Match(line);
                Match playerEntityMatch = RegexManager.playerEntityRegex.Match(line);
                Match playerIdMatch = RegexManager.playerIdRegex.Match(line);
                Match mulliganMatch = RegexManager.mulliganRegex.Match(line);
                Match gameResultMatch = RegexManager.gameResultRegex.Match(line);

                if (heroMatch.Success)
                {
                    int entityId = Int32.Parse(heroMatch.Groups[1].Value);
                    int heroId = Int32.Parse(heroMatch.Groups[2].Value) - 1;
                    _heroEntityDict.Add(entityId, Hero.HeroIdList[heroId]);
                }

                else if (playerEntityMatch.Success)
                {
                    string username = playerEntityMatch.Groups[1].Value;
                    int entityId = Int32.Parse(playerEntityMatch.Groups[2].Value);
                    _playerEntityDict.Add(simplifyString(username), entityId);
                }

                else if (playerIdMatch.Success)
                {
                    string username = playerIdMatch.Groups[1].Value;
                    int playerId = Int32.Parse(playerIdMatch.Groups[2].Value);
                    _playerIdDict.Add(playerId, simplifyString(username));
                }

                else if (mulliganMatch.Success)
                {
                    if (myId < 0) myId = Int32.Parse(mulliganMatch.Groups["player"].Value);
                }

                else if (gameResultMatch.Success)
                {
                    count++;
                    string username = gameResultMatch.Groups[1].Value;
                    string result = gameResultMatch.Groups[2].Value;
                    if (_playerEntityDict.ContainsKey(simplifyString(username)))
                    {
                        if (_playerIdDict[myId] == simplifyString(username))
                        {
                            playerJson["name"] = username;
                            playerJson["hero"] = _heroEntityDict[_playerEntityDict[simplifyString(username)]];
                            playerJson["result"] = GameResult.dictionary[result];
                            
                            resultJson["player"] = playerJson;
                        }
                        else
                        {
                            opponentJson["name"] = username;
                            opponentJson["hero"] = _heroEntityDict[_playerEntityDict[simplifyString(username)]];
                            opponentJson["result"] = GameResult.dictionary[result];

                            resultJson["opponent"] = opponentJson;
                        }
                    }

                    if (count % 2 == 0)
                    {
                        if (!resultJson.Empty())
                        {
                            resultJson["date"] = DateTime.Now.ToLocalTime().ToString().Replace("/", "-");
                            Request.Post(resultJson.ToString());
                        }
                        resultJson.Clear();
                        playerJson.Clear();
                        opponentJson.Clear();
                        _playerEntityDict.Clear();
                        _heroEntityDict.Clear();
                        _playerIdDict.Clear();
                    }
                }
            }
            copyGameLogLines.Clear();
            barrier.SignalAndWait();
        }

        private string simplifyString(string str)
        {
            return str.Replace(' ', '_').ToLower();
        }
    }
}
