using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SSHMan
{
    public class SSHToProfileConverter
    {
        public string WtSettings { get; private set; }
        public SSHToProfileConverter()
        {
            WtSettings = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.WindowsTerminal_8wekyb3d8bbwe", "LocalState", "settings.json");
        }

        public string GenerateSettings()
        {
            var json = File.ReadAllText(WtSettings);
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

            return cfg.ToJson();

        }
    }
}
