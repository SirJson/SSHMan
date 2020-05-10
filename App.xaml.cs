using System;
using Serilog;
using Serilog.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Threading;
using ControlzEx.Standard;

namespace SSHMan
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
    }
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DataPath, LogPath, ScriptPath, ModulePath;
        static readonly Mutex singleAppMutex = new Mutex(true, "{14E68EB5-8922-4752-B03C-D3D672135FA6}");

        public static void Panic(string message)
        {
            _ = MessageBox.Show($"PANIC: {message}", "Panic", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(22);
        }

        private void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }
        }

        private void InstallAssets() {
            Log.Debug("Installing {file}", ScriptPath);
            File.WriteAllBytes(ScriptPath, Scripts.sshchild);
            //var modPath = Environment.GetEnvironmentVariable("PSModulePath").Split(";").First();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (singleAppMutex.WaitOne(TimeSpan.Zero, true))
            {
                DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SSHMan");
                ScriptPath = Path.Combine(DataPath, "sshchild.ps1");
                ModulePath = Path.Combine(DataPath, "ReadNamedPipeCmdlet.dll");
                EnsureDirectory(DataPath);
                InstallAssets();
                LogPath = Path.Combine(DataPath, "sshman.log");
                if (e.Args.Length > 0 && e.Args[0] == "-d")
                {
                    DebugLogger();
                }
                else
                {
                    StandardLogger();
                }
            }
            else
            {
                this.Shutdown();
            }
        }

        private void DebugLogger()
        {
            if (!Kernel32.AllocConsole())
            {
                Panic("Failed to allocate console");
            }

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File(LogPath, rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }

        private void StandardLogger()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.File(LogPath, rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }
    }


}
