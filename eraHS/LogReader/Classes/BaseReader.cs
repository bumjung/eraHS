using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using eraHS.Constants.Hearthstone;
using eraHS.Utility;

namespace eraHS.LogReader.Classes
{
    abstract class BaseReader
    {
        private LogWatcher _logWatcher;

        private string _logFilePath;
        private long _offset;
        private BinarySemaphore _sem;
        private int _matchIndex;

        protected List<Regex> _regexList;
        protected string _logFileName;

        public List<String> LogLines { get; set; }
        public List<String> CopyLogLines { get; set; }

        protected void init()
        {
            this.LogLines = new List<String>();
            this.CopyLogLines = new List<String>();

            _logFilePath = Config.userFilePath + _logFileName;
            _offset = 0;
            _sem = new BinarySemaphore(0, 1);
            _matchIndex = 0;

            File.WriteAllText(_logFilePath, String.Empty);
            _logWatcher = new LogWatcher(_logFileName.Substring(1), ref _sem);
            _logWatcher.start();

            this._regexList = new List<Regex>();
        }

        public abstract void parseLogLines(Json resultJson);

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
                            this.LogLines.Add(line);

                            Match match = _regexList[_matchIndex].Match(line);
                            if (match.Success)
                            {
                                _matchIndex++;
                            }

                            _offset += Encoding.UTF8.GetByteCount(line + Environment.NewLine);

                            if (_matchIndex == _regexList.Count)
                            {
                                _matchIndex = 0;
                                LogManager.barrier.SignalAndWait();
                                return;
                            }
                        }
                        _sem.WaitOne();
                    }
                }
            }
        }
    }
}
