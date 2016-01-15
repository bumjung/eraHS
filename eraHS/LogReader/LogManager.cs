using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using eraHS.Constants.Hearthstone;
using eraHS.Utility;
using eraHS.Utility.RegexHelper;

namespace eraHS.LogReader
{
    class LogManager
    {
        private LogReader _logReader;
        private Dictionary<int, string> _heroEntityDict;
        private Dictionary<string, int> _playerEntityDict;
        private Dictionary<int, string> _playerIdDict;
        private FileSystemWatcher watcher;
        private int watcherCount;

        public static Barrier sync;
        public static BinarySemaphore sem;

        public List<String> gameLogLines;
        public List<String> copyGameLogLines;

        public LogManager()
        {
            gameLogLines = new List<String>();
            copyGameLogLines = new List<String>();

            _logReader = new LogReader(gameLogLines);
            _heroEntityDict = new Dictionary<int, string>();
            _playerEntityDict = new Dictionary<string, int>();
            _playerIdDict = new Dictionary<int, string>();

            sync = new Barrier(participantCount: 2);
            sem = new BinarySemaphore(0, 1);

        }

        public void start()
        {
            while (true)
            {
                var readingLogFile = new Thread(() =>
                {
                    this.watcherInit();
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

        private void watcherInit()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = Config.userFilePath;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                    | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = @"Power*.log";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (++watcherCount % 2 == 0) sem.ReleaseOne();
        }

        private void parseLogLines()
        {
            int count = 0;
            int myId = -1;
            foreach (string line in copyGameLogLines)
            {
                Match heroMatch = RegexManager.heroIdRegex.Match(line);
                if (heroMatch.Success)
                {
                    int entityId = Int32.Parse(heroMatch.Groups[1].Value);
                    int heroId = Int32.Parse(heroMatch.Groups[2].Value) - 1;
                    _heroEntityDict.Add(entityId, Hero.HeroIdList[heroId]);
                }

                Match playerEntityMatch = RegexManager.playerEntityRegex.Match(line);
                if (playerEntityMatch.Success)
                {
                    string username = playerEntityMatch.Groups[1].Value;
                    int entityId = Int32.Parse(playerEntityMatch.Groups[2].Value);
                    _playerEntityDict.Add(simplifyString(username), entityId);
                }

                Match playerIdMatch = RegexManager.playerIdRegex.Match(line);
                if (playerIdMatch.Success)
                {
                    string username = playerIdMatch.Groups[1].Value;
                    int playerId = Int32.Parse(playerIdMatch.Groups[2].Value);
                    _playerIdDict.Add(playerId, simplifyString(username));
                }

                Match mulliganMatch = RegexManager.mulliganRegex.Match(line);
                if (mulliganMatch.Success)
                {
                    if (myId < 0) myId = Int32.Parse(mulliganMatch.Groups["player"].Value);
                }

                Match gameResultMatch = RegexManager.gameResultRegex.Match(line);
                if (gameResultMatch.Success)
                {
                    count++;
                    string username = gameResultMatch.Groups[1].Value;
                    string result = gameResultMatch.Groups[2].Value;
                    if (_playerEntityDict.ContainsKey(simplifyString(username)))
                    {
                        string output = username + '\t'
                            + _heroEntityDict[_playerEntityDict[simplifyString(username)]]
                            + '\t' + result;
                        
                        if (_playerIdDict[myId] == simplifyString(username))
                        {
                            output += '\t' + "ME";
                        }

                        Console.WriteLine(output);
                    }

                    if (count % 2 == 0)
                    {
                        _playerEntityDict.Clear();
                        _heroEntityDict.Clear();
                        _playerIdDict.Clear();
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
