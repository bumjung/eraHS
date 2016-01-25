using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using eraHS.Constants.Hearthstone;
using eraHS.Utility;
using eraHS.Utility.RegexHelper;

namespace eraHS.LogReader.Classes
{
    class PowerReader : BaseReader
    {
        int _gameResultMatchCount;
        int _myId;

        private Dictionary<int, string> _heroEntityDict;
        private Dictionary<string, int> _playerEntityDict;
        private Dictionary<int, string> _playerIdDict;

        public PowerReader()
        {
            _logFileName = @"/Power.log";
            base.init();
            _regexList.Add(RegexManager.createGameRegex);
            _regexList.Add(RegexManager.goldRewardRegex);

            _gameResultMatchCount = 0;
            _myId = -1;
            _heroEntityDict = new Dictionary<int, string>();
            _playerEntityDict = new Dictionary<string, int>();
            _playerIdDict = new Dictionary<int, string>();
        }

        public override void parseLogLines(Json resultJson)
        {
            this.newGame();
            DateTime startDate = DateTime.Now;

            foreach (string line in this.CopyLogLines)
            {
                Match createGameMatch = RegexManager.createGameRegex.Match(line);
                Match heroMatch = RegexManager.heroIdRegex.Match(line);
                Match playerEntityMatch = RegexManager.playerEntityRegex.Match(line);
                Match playerIdMatch = RegexManager.playerIdRegex.Match(line);
                Match mulliganMatch = RegexManager.mulliganRegex.Match(line);
                Match gameResultMatch = RegexManager.gameResultRegex.Match(line);

                if (createGameMatch.Success)
                {
                    string strStartDate = createGameMatch.Groups["startTime"].Value;
                    startDate = strStartDate.ConvertStringToDateTime();
                }
                else if (heroMatch.Success)
                {
                    int entityId = Int32.Parse(heroMatch.Groups["entityId"].Value);
                    int heroId = Int32.Parse(heroMatch.Groups["heroId"].Value) - 1;
                    _heroEntityDict.Add(entityId, Hero.HeroIdList[heroId]);
                }
                else if (playerEntityMatch.Success)
                {
                    string username = playerEntityMatch.Groups["name"].Value;
                    int entityId = Int32.Parse(playerEntityMatch.Groups["entityId"].Value);
                    _playerEntityDict.Add(username.Simplify(), entityId);
                }
                else if (playerIdMatch.Success)
                {
                    string username = playerIdMatch.Groups["name"].Value;
                    int playerId = Int32.Parse(playerIdMatch.Groups["playerId"].Value);
                    _playerIdDict.Add(playerId, username.Simplify());
                }
                else if (mulliganMatch.Success)
                {
                    if (_myId < 0) _myId = Int32.Parse(mulliganMatch.Groups["playerId"].Value);
                }
                else if (gameResultMatch.Success)
                {
                    _gameResultMatchCount++;
                    string username = gameResultMatch.Groups["name"].Value;
                    string result = gameResultMatch.Groups["result"].Value;
                    string strEndDate = gameResultMatch.Groups["endTime"].Value;

                    this.populatePlayerOpponent(resultJson, username, result);

                    if (isGameEnded() && !resultJson.Empty())
                    {
                        resultJson["startDate"] = startDate.ToString().Replace("/", "-");
                        resultJson["endDate"] = strEndDate.ConvertStringToDateTime().ToString().Replace("/", "-");
                    }
                }
            }
        }

        private void newGame()
        {
            _gameResultMatchCount = 0;
            _myId = -1;
            _playerEntityDict.Clear();
            _heroEntityDict.Clear();
            _playerIdDict.Clear();
        }

        private bool isGameEnded()
        {
            return _gameResultMatchCount % 2 == 0;
        }

        private void populatePlayerOpponent(Json resultJson, string username, string result)
        {
            Json playerJson = new Json();
            Json opponentJson = new Json();

            if (_playerEntityDict.ContainsKey(username.Simplify()))
            {
                if (_playerIdDict[_myId] == username.Simplify())
                {
                    playerJson["name"] = username;
                    playerJson["hero"] = _heroEntityDict[_playerEntityDict[username.Simplify()]];
                    playerJson["result"] = GameResult.dictionary[result];

                    resultJson["player"] = playerJson;
                }
                else
                {
                    bool isComputer = !_heroEntityDict.ContainsKey(_playerEntityDict[username.Simplify()]);
                    opponentJson["name"] = isComputer ? "Computer" : username;
                    opponentJson["hero"] = isComputer ? username : _heroEntityDict[_playerEntityDict[username.Simplify()]];
                    opponentJson["result"] = GameResult.dictionary[result];

                    resultJson["opponent"] = opponentJson;
                }
            }
        }
    }
}
