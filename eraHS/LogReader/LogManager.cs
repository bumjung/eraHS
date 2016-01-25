using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using eraHS.Utility;
using eraHS.LogReader.Classes;

namespace eraHS.LogReader
{
    class LogManager
    {
        private PowerReader _powerReader;
        private ModeReader _modeReader;

        public static Barrier barrier;

        public LogManager()
        {
            _powerReader = new PowerReader();
            _modeReader = new ModeReader();

            barrier = new Barrier(participantCount: 3);

        }

        public void start()
        {
            while (true)
            {
                var readingPowerThread = new Thread(() =>
                {
                    _powerReader.readLogFile();
                });
                readingPowerThread.Start();

                var readingModeThread = new Thread(() =>
                {
                    _modeReader.readLogFile();
                });
                readingModeThread.Start();

                var parsingThread = new Thread(() =>
                {
                    this.parseAndSendResults();
                });
                parsingThread.Start();

                readingPowerThread.Join();
                readingModeThread.Join();
                parsingThread.Join();

                _powerReader.CopyLogLines.ShallowCopy(_powerReader.LogLines);
                _modeReader.CopyLogLines.ShallowCopy(_modeReader.LogLines);

                _powerReader.LogLines.Clear();
                _modeReader.LogLines.Clear();
            }
        }

        private void parseAndSendResults()
        {
            Json resultJson = new Json();
            _powerReader.parseLogLines(resultJson);

            if (!resultJson.Empty())
            {
                Logger.log("Sending Request");

                _modeReader.parseLogLines(resultJson);
                Request.Post(resultJson.ToString());
            }

            _powerReader.CopyLogLines.Clear();
            _modeReader.CopyLogLines.Clear();
            barrier.SignalAndWait();
        }
    }
}
