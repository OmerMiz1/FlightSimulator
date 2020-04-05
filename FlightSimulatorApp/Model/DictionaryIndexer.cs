using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlightSimulatorApp.Model {
    public class DictionaryIndexer {
        private Dictionary<string, string> dic;
        public string this[string key] {
            get => dic[key];
            set {
                if (key != null && dic[key] != value)
                {
                    dic[key] = value;
                }
            }
        }

    }
}