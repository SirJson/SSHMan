using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Environment;

namespace SSHMan {

    public class SSHParser {
        const string ConfigPattern = @"(Host\s[\w\*]+)(\n\s+\w+\s.*)+";
        const string UserSSHConfig = ".ssh/config";
        const string CfgKeyHost = "hostname";
        const string CfgKeyPort = "port";

        public struct ConfigEntry {
            public string Host;
            public Dictionary<string, string> Data;

            public SSHHostEntry ToEntry()
            {
                var ip = Data.Keys.Contains(CfgKeyHost) ? Data[CfgKeyHost] : "-";
                var port = Data.Keys.Contains(CfgKeyPort) ? Data[CfgKeyPort] : "22";
                return new SSHHostEntry()
                {
                    Name = Host,
                    Address = $"{ip}:{port}"
                };
            }
        }

        public Dictionary<string, ConfigEntry> HostMap () {
            var localsshdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
            var localsshcfg = Path.Combine(localsshdir, "config");
            App.EnsureDirectory(localsshdir);

            if (!File.Exists(localsshcfg))
            {
                File.WriteAllText(localsshcfg, Scripts.example_ssh);
            }

            var output = new Dictionary<string, ConfigEntry> ();
            var home = GetFolderPath (SpecialFolder.UserProfile);
            var input = File.ReadAllText (Path.Join (home, UserSSHConfig));

            var options = RegexOptions.Multiline | RegexOptions.CultureInvariant;

            foreach (Match m in Regex.Matches (input, ConfigPattern, options)) {
                var entry = new ConfigEntry() { Data = new Dictionary<string, string>()};
                var data = m.Value.Split('\n');
                var host = data[0].Split("Host")[1].Trim();
                if(host == "*") continue;
                entry.Host = host;
                for (var i = 1; i < data.Length; i++)
                {
                    (var key, var value, _) = data[i].Trim().Split(' ');
                    entry.Data.Add(key.ToLowerInvariant(), value);
                }
                output[entry.Host] = entry;
            }

            return output;
        }
    }
}