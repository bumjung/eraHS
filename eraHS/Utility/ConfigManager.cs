using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eraHS.Utility
{
    class ConfigManager
    {
        public void init()
        {
            string configContent = "[Power]\nLogLevel=1\nFilePrinting=true\nConsolePrinting=true\nScreenPrinting=false\n\n[LoadingScreen]\nLogLevel=1\nFilePrinting=true\nConsolePrinting=true\nScreenPrinting=false";

            if (!System.IO.File.Exists(Config.configFilePath))
            {
                System.IO.File.WriteAllText(Config.configFilePath, configContent);
            }
        }
    }
}
