using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher {
    public static class IPLocate {

        private static List<Tuple<uint, uint, string>> IpRanges;

        public static void Init() {
            IpRanges = new List<Tuple<uint, uint, string>>();

            string content = string.Empty;
            using(var sr = new StreamReader("iploc.csv")) {
                sr.ReadLine();

                var cur = string.Empty;
                while((cur = sr.ReadLine()) != null) {
                    var vals = cur.Replace("\"", "").Split(',');
                    var inFrom = vals[0];
                    var inTo = vals[1];
                    var inName = vals[3];

                    IpRanges.Add(new Tuple<uint, uint, string>(uint.Parse(inFrom), uint.Parse(inTo), inName));
                }
            }
        }

        public static string GetIpCountry(string ip) {
            var bytes = ip.Split('.').Select(s => (byte)int.Parse(s)).ToArray();
            return GetIpCountry(bytes);
        }

        public static string GetIpCountry(byte[] ip) {
            var converted = BitConverter.ToUInt32(ip.Reverse().ToArray(), 0);

            foreach(var entry in IpRanges) {
                if(converted >= entry.Item1 && converted < entry.Item2) {
                    return entry.Item3;
                }
            }

            return string.Empty;
        }
    }
}
