using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;

namespace SSHMan
{
    public static class SSHToProfileConverter
    {
        public static (bool,string) GenerateSettings(string file)
        {
            if(!File.Exists(file)) {
                Log.Warning("Skipped terminal settings update because '{file}' doesn't exist", file);
                return (false,string.Empty); // If the file doesn't exist that's okay
            }
            var json = File.ReadAllText(file);
            var cfg = TerminalConfig.FromJson(json);
            var hosts = SSHParser.MapHosts();
            if (cfg.Profiles.HasValue)
            {
                var profileMaster = cfg.Profiles.Value;
                var profiles = profileMaster.ProfilesObject;

                foreach (var host in hosts)
                {
                    var name = $"{host.Key} (SSH)";
                    if (profiles.List.Any((x) => x.Name == name))
                    {
                        continue;
                    }
                    var list = new ProfileList()
                    {
                        Guid = "{" + Guid.NewGuid().ToString() + "}",
                        Commandline = $"ssh {host.Key}",
                        Name = name,
                        CloseOnExit = CloseOnExitEnum.Always
                    };
                    profiles.List.Add(list);
                }
                cfg.Profiles = profiles;
            }

            return (true,cfg.ToJson());

        }
    }
}
