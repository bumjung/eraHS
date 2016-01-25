using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eraHS
{
    public class Config
    {
        public static readonly string userFilePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%");
        public static readonly string programFilePath = Environment.GetEnvironmentVariable(@"ProgramFiles(x86)");

        public static readonly string configFilePath = userFilePath + @"\AppData\Local\Blizzard\Hearthstone\log.config";
        public static readonly string hsLogDirPath = Config.programFilePath + @"/Hearthstone/Logs";

        public static readonly int timeToWaitForMultipleLogLines = 5000;
    }
}
