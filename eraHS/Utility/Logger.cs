using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eraHS.Utility
{
    static class Logger
    {
        private static List<String> _logData;
        private static LogWindow _lw;

        static Logger()
        {
            _logData = new List<String>();
        }

        public static void log(String item)
        {
            _logData.Add(DateTime.Now + ": " + item);
            if(_lw != null) {
                _lw.Invoke((Action)delegate{
                    _lw.update();
                });
            }
        }

        public static string toString()
        {
            string temp = "";
            foreach (string data in _logData)
            {
                temp += data + '\n';
            }

            return temp;
        }

        public static void newWindow(LogWindow lw)
        {
            _lw = lw;
        }
    }
}
