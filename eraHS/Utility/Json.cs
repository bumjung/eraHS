using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eraHS.Utility
{
    class Json
    {
        private Dictionary<string, string> dictionary;

        public Json()
        {
            dictionary = new Dictionary<string,string>();
        }

        public override string ToString()
        {
            var output = "";
            foreach(KeyValuePair<string, string> pair in dictionary)
            {
                output += "\"" + pair.Key + "\"" + ":";
                output += pair.Value[0] == '{' ? pair.Value + "," : "\"" + pair.Value + "\",";
            }
            output = output.Length > 0 ? output.Remove(output.Length - 1) : output;
            return "{" + output + "}";
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }

        public bool Empty()
        {
            return dictionary.Count == 0;
        }

        public dynamic this[string key]
        {
            get { return dictionary[key]; }
            set {
                dictionary[key] = typeof(string) == value.GetType() ? value : value.ToString();
            }
        }
    }
}
