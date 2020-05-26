using System;
using System.Collections.Generic;
using System.Linq;

namespace SSHMan {

    public struct ConfigEntry : IEquatable<ConfigEntry> {
        public string Host { get; set; }
        public Dictionary<string, string> Data { get; private set; }

        const string CfgKeyHost = "hostname";
        const string CfgKeyPort = "port";

        public static ConfigEntry Create (string host, string port) {
            return new ConfigEntry () {
                    Host = host,
                    Data = new Dictionary<string, string> () { { CfgKeyHost, host }, { CfgKeyPort, port },
                }
            };
        }

        public static ConfigEntry Create () {
            return new ConfigEntry () {
                    Host = "Unknown",
                    Data = new Dictionary<string, string> () { { CfgKeyHost, "Unknown" } }
            };
        }

        public override int GetHashCode () {
            return HashCode.Combine (Host, Data);
        }

        public SSHHostEntry ToEntry () {
            var ip = Data.Keys.Contains (CfgKeyHost) ? Data[CfgKeyHost] : "-";
            var port = Data.Keys.Contains (CfgKeyPort) ? Data[CfgKeyPort] : "22";
            return new SSHHostEntry () {
                Name = Host,
                    Address = $"{ip}:{port}"
            };
        }

        public static bool operator == (ConfigEntry left, ConfigEntry right) {
            return left.Equals (right);
        }

        public static bool operator != (ConfigEntry left, ConfigEntry right) {
            return !(left == right);
        }

        public bool Equals (ConfigEntry other) {
            return other is ConfigEntry entry &&
                Host == entry.Host &&
                EqualityComparer<Dictionary<string, string>>.Default.Equals (Data, entry.Data);
        }

        public override bool Equals(object obj)
        {
            return obj is ConfigEntry entry && Equals(entry);
        }
    }
}