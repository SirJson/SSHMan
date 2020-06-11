using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Environment;

namespace SSHMan
{

    public static class SSHParser {
        const string ConfigPattern = @"(Host\s[\w\*]+)(\n\s+\w+\s.*)+";
        const string UserSSHConfig = ".ssh/config";

        public static Dictionary<string, ConfigEntry> MapHosts () {
            var localsshdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
            var localsshcfg = Path.Combine(localsshdir, "config");
            App.EnsureDirectory(localsshdir);

            if (!File.Exists(localsshcfg))
            {
                File.WriteAllText(localsshcfg, Encoding.UTF8.GetString(Scripts.ssh));
            }

            var output = new Dictionary<string, ConfigEntry> ();
            var home = GetFolderPath (SpecialFolder.UserProfile);
            var input = File.ReadAllText (Path.Join (home, UserSSHConfig));

            var options = RegexOptions.Multiline | RegexOptions.CultureInvariant;

            foreach (Match m in Regex.Matches (input, ConfigPattern, options)) {
                var entry = ConfigEntry.Create();
                var data = m.Value.Split('\n');
                var host = data[0].Split("Host")[1].Trim();
                if(host == "*") continue;
                entry.Host = host;
                for (var i = 1; i < data.Length; i++)
                {
                    (var key, var value, _) = data[i].Trim().Split(' ');
                    entry.Data.Add(key.ToUpperInvariant(), value);
                }
                output[entry.Host] = entry;
            }

            return output;
        }
    }
}
