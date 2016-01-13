using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eraHS
{
    class Config
    {
        public static readonly string userFilePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%");
        public static readonly string configFilePath = userFilePath + @"\log.config";


    }
}
