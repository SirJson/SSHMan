using System;
using System.IO;
using System.IO.Pipes;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;

namespace SSHChild {
    [Cmdlet (VerbsCommunications.Read, "NamedPipe")]
    [OutputType (typeof (string))]
    public class ReadNamedPipeCommand : PSCmdlet {
        [Parameter (
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string PipeName { get; set; }

       [Parameter (
            Mandatory = false,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int Timeout { get; set; }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord () {
            string output = "";
            using (var pipeClient = new NamedPipeClientStream (".",PipeName,PipeDirection.InOut,PipeOptions.None,TokenImpersonationLevel.Impersonation)) {

                try {
                    pipeClient.Connect(Timeout);
                    var stream = new StreamString (pipeClient);
                    output = stream.ReadString();
                } catch (IOException e) {
                    WriteError (new ErrorRecord (e, "pipeError", ErrorCategory.InvalidOperation, this));
                }
                finally {
                    pipeClient.Dispose();
                }
                WriteObject (output);
            }
        }
    }

}