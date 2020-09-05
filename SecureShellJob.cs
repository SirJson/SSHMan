using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Serilog;

namespace SSHMan
{
    class SecureShellJob
    {
        public readonly Guid ID;
        private Thread worker;
        private readonly string hostname;

        public SecureShellJob(string host)
        {
            ID = Guid.NewGuid();
            hostname = host;
        }

        public void StartSession()
        {
            worker = new Thread(() => this.SessionThread(hostname));
        }

        private void SessionThread(string target)
        {
            Log.Information("Connecting to {host}", target);

            var wtpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "WindowsApps", "wt.exe");
            Log.Information("Executing client script...");
            using Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = wtpath,
                    ArgumentList = { "new-tab", "pwsh.exe", "-NoLogo", "-NonInteractive", "-File", App.ScriptPath, ID.ToString() },
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.Unicode,
                },
                EnableRaisingEvents = true,

            };
            proc.Exited += this.Proc_Exited;
            proc.ErrorDataReceived += this.Proc_ErrorDataReceived;
            var success = proc.Start();
            Debug.Assert(success);
            Log.Debug("Thread ({thread}) initialized and running", Thread.CurrentThread.ManagedThreadId);
            proc.WaitForExit();
            Log.Debug("Process for thread ({thread}) exited", Thread.CurrentThread.ManagedThreadId);
        }

        private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e) => Log.Error("SSH Error: {error}", e.Data);

        private void Proc_Exited(object sender, EventArgs e)
        {
            if (sender is Process handle)
            {
                if (handle.ExitCode != 0)
                {
                    var err = handle.StandardError.ReadToEnd();
                    _ = MessageBox.Show(err, LangDefault.ConnectionFail, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                var workId = new Guid(handle.StartInfo.ArgumentList.Last());
                Log.Debug("Thread with work id {id} died and was added to the queue", workId);
                App.AnnounceSessionEnd(workId);
            }
        }
    }
}
