using System;
using System.IO;

using eraHS.Utility;

namespace eraHS.LogReader
{
    class LogWatcher
    {
        private FileSystemWatcher _watcher;
        private BinarySemaphore _sem;
        private int _watcherCount;
        private string _configFilePath;

        public LogWatcher(string configFilePath, ref BinarySemaphore sem)
        {
            _watcher = new FileSystemWatcher();
            _sem = sem;
            _watcherCount = 0;
            _configFilePath = configFilePath;
        }

        public void start()
        {
            _watcher.Path = Config.userFilePath;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                    | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _watcher.Filter = _configFilePath;
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (++_watcherCount % 2 == 0) _sem.ReleaseOne();
        }
    }
}
