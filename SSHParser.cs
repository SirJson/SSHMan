using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Serilog;
using static System.Environment;

namespace SSHMan
{

    public static class SSHParser
    {
        const string ConfigPattern = @"(^Host\s[\w\*]+)(\n\s+\w+\s.*)+";
        const string UserSSHConfig = ".ssh/config";

        public static Dictionary<string, ConfigEntry> MapHosts()
        {
            var output = new Dictionary<string, ConfigEntry>();
            var cfgpath = Path.Join(GetFolderPath(SpecialFolder.UserProfile), UserSSHConfig);
            Log.Debug("Reading ssh config from {path}",cfgpath);
            var input = File.ReadAllText(cfgpath);
            var options = RegexOptions.Multiline | RegexOptions.CultureInvariant;
            input = input.Trim().Replace("\r\n","\n", true, CultureInfo.InvariantCulture);
            var matches = Regex.Matches(input, ConfigPattern, options);
            if(matches.Count == 0) Log.Warning("No SSH hosts found");
            foreach (Match m in matches)
            {
                Log.Debug("Matched config entry",m.Value);
                var entry = ConfigEntry.Create();
                var data = m.Value.Split('\n');
                var host = data[0].Split("Host")[1].Trim();
                if (host == "*") continue;
                entry.Host = host;
                for (var i = 1; i < data.Length; i++)
                {
                    (var key, var value, _) = data[i].Trim().Split(' ');
                    var exactKey = key.ToUpperInvariant();
                    if (entry.Data.ContainsKey(exactKey))
                    {
                        entry.Data[exactKey] = value;
                    }
                    else {
                        entry.Data.Add(exactKey, value);
                    }
                }
                output[entry.Host] = entry;
            }

            return output;
        }
    }
}
