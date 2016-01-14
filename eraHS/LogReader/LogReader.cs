using eraHS.Constants.Hearthstone;
using eraHS.Utility.RegexHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace eraHS.LogReader
{
    class LogReader
    {
        private string _logFilePath;
        private int _offset;
        List<String> gameLogLines;

        public LogReader(List<String> lines)
        {
            _logFilePath = Config.userFilePath + @"\Power.log";
            _offset = 0;
            gameLogLines = lines;
        }

        public void readLogFile()
        {
            using(FileStream fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fileStream.Seek(_offset, SeekOrigin.Begin);
                // TODO: end of file, not equal...
                while (fileStream.Length == _offset)
                {
                    LogManager.sem.WaitOne();
                }

                using (var streamReader = new StreamReader(fileStream))
                {
                    string line;
                    while (!streamReader.EndOfStream && (line = streamReader.ReadLine()) != null)
                    {
                        gameLogLines.Add(line);
                        Match endGameMatch = RegexManager.goldRewardRegex.Match(line);

                        _offset += Encoding.UTF8.GetByteCount(line + Environment.NewLine);
                        if (endGameMatch.Success) {
                            LogManager.sync.SignalAndWait();
                            return;
                        }
                    }
                }
            }
        }
    }
}
