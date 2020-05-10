using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Buffers.Text;
using System.IO.Pipes;
using System.Windows;
using System.Threading;
using System.Collections.Concurrent;
using System.Security.Principal;
using Serilog;

namespace SSHMan
{
    public class MessageItem
    {
        public Guid Id;
        public string Data;
        public bool LastMessage;
    }

    public class PowerShellIPC
    {
        public static MessageItem Message(string message, bool exitAfterDelivery, Guid id)
        {
            Log.Information("IPC Message out: {msg}",message);
            var item = new MessageItem()
            {
                Id = id,
                Data = message,
                LastMessage = exitAfterDelivery
            };
            var success = ThreadPool.QueueUserWorkItem(ProvideMessage, item);
            if(!success)
            {
                _ = MessageBox.Show("Failed to spawn message thread", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return item;
        }

        public static void ProvideMessage(object payload)
        {
            var message = (MessageItem)payload;
            var pipename = $"sshmancom-{message.Id}";
            var pipeServer = new NamedPipeServerStream(pipename, PipeDirection.InOut, 1);
            Log.Information("Worker started offering message on '.{path}.' Waiting for connection...", pipename);
            pipeServer.WaitForConnection();
            try
            {
                Log.Information("Client connected!");
                var stream = new StreamString(pipeServer);
                var bytesWritten = stream.WriteString(message.Data);
                Log.Information("Transfered ipc message to {user}", pipeServer.GetImpersonationUserName());
                if(message.LastMessage)
                {
                    MainWindow.ShutdownSignal.Set();
                }
            }
            catch (IOException e)
            {
                _ = MessageBox.Show(string.Format("ERROR: {0}", e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                pipeServer.Dispose();
            }
        }
    }
}
