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
        private string _logFileName;
        private string _logFilePath;
        private long _offset;
        List<String> gameLogLines;

        public LogReader(List<String> lines)
        {
            _logFileName = @"\Power_1.log";
            _logFilePath = Config.userFilePath + _logFileName;
            _offset = 0;
            gameLogLines = lines;
        }

        public void readLogFile()
        {
            while (true)
            {
                using (FileStream fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileStream.Seek(_offset, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(fileStream))
                    {
                        string line = "";
                        while (!streamReader.EndOfStream && (line = streamReader.ReadLine()) != null)
                        {
                            gameLogLines.Add(line);
                            Match endGameMatch = RegexManager.goldRewardRegex.Match(line);

                            _offset += Encoding.UTF8.GetByteCount(line + Environment.NewLine);
                            if (endGameMatch.Success)
                            {
                                LogManager.barrier.SignalAndWait();
                                return;
                            }
                        }
                        LogManager.sem.WaitOne();
                    }
                }
            }
        }
    }
}
