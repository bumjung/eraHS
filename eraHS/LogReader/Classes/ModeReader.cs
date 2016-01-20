using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using eraHS.Utility;
using eraHS.Utility.RegexHelper;

namespace eraHS.LogReader.Classes
{
    class ModeReader : BaseReader
    {

        public ModeReader()
        {
            _logFileName = @"/LoadingScreen.log";
            base.init();
            _regexList.Add(RegexManager.gameModeRegex);
            _regexList.Add(RegexManager.gameplayStartRegex);
        }

        public override void parseLogLines(Json resultJson)
        {
            for (int i = this.CopyLogLines.Count - 1; i >= 0; i--)
            {
                Match gameModeMatch = RegexManager.gameModeRegex.Match(this.CopyLogLines[i]);
                if (gameModeMatch.Success)
                {
                    resultJson["mode"] = this.convertMode(gameModeMatch.Groups["currMode"].Value);
                    return;
                }
            }
        }

        private string convertMode(string mode)
        {
            switch (mode)
            {
                case "FRIENDLY":
                    return "Friendly";
                case "DRAFT":
                    return "Arena";
                case "TAVERN_BRAWL":
                    return "Brawl";
                case "ADVENTURE":
                    return "Practice";
                case "TOURNAMENT":
                    return "Casual";
                default:
                    return "Unknown";
            }
        }
    }
}
