using System;
using System.IO;

namespace eraHS.LogReader
{
    public class LogWatcher
    {
        private FileSystemWatcher _watcher;
        private int _watcherCount;
        private string _configFilePath;

        public LogWatcher(string configFilePath)
        {
            _watcher = new FileSystemWatcher();
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
            if (++_watcherCount % 2 == 0) LogManager.sem.ReleaseOne();
        }
    }
}
