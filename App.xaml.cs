using System;
using Serilog;

using System.Windows;
using System.IO;
using System.Threading;


namespace SSHMan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DataPath { get; private set; }
        public static string ScriptPath { get; private set; }
        public static string LogPath { get; private set; }
        static readonly Mutex singleAppMutex = new Mutex(true, "{529A6125-B42E-49A8-B289-216D8FFE45B8}");

        public static void Panic(string message)
        {
            _ = MessageBox.Show($"PANIC: {message}", "Panic", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(22);
        }

        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }
        }

        private static void InstallIfNotExists(string file, byte[] data)
        {
            if(File.Exists(file)) return;
            Log.Information("Installing {file}", file);
            File.WriteAllBytes(file, data);
        }

        private static void InstallAssets() {
            var modDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"PowerShell","Modules","ReadNamedPipe");
            var modDefinition = Path.Combine(modDir, "ReadNamedPipe.psd1");
            var modAssembly = Path.Combine(modDir, "ReadNamedPipeCmdlet.dll");
            EnsureDirectory(modDir);
            InstallIfNotExists(ScriptPath,Scripts.sshchild);
            InstallIfNotExists(modDefinition,Scripts.ModuleDefinition);
            InstallIfNotExists(modAssembly,Scripts.ReadNamedPipeCmdlet);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            if (singleAppMutex.WaitOne(TimeSpan.Zero, true))
            {
                DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SSHMan");
                ScriptPath = Path.Combine(DataPath, "sshchild.ps1");
                LogPath = Path.Combine(DataPath, "sshman.log");

                EnsureDirectory(DataPath);

                if (e.Args.Length > 0 && e.Args[0] == "-d")
                {
                    DebugLogger();
                }
                else
                {
                    StandardLogger();
                }

                InstallAssets();
            }
            else
            {
                var hwnd = NativeMethods.FindWindow(null,"SSHMan");
                NativeMethods.ShowWindow(hwnd,NativeMethods.Win32ShowCmd.Restore);
                NativeMethods.SetForegroundWindow(hwnd);
                this.Shutdown();
            }
        }

        internal static void AnnounceSessionEnd(Guid workId)
        {
            throw new NotImplementedException();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // If we exit because of any other reason than the happy path the mutex might still exist so we better kill it here
            singleAppMutex.Close();
            singleAppMutex.Dispose();
        }

        private static void DebugLogger()
        {
            if (!NativeMethods.AllocConsole())
            {
                Panic("Failed to allocate console");
            }

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File(LogPath, rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }

        private static void StandardLogger()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.File(LogPath, rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }
    }


}
