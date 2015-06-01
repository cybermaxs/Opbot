using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System;

namespace Opbot.Core.Tasks
{
    class JpegTask : BaseCmdTask, IPipelineTask<Message, Message>
    {
        public JpegTask(Options options, ILogService logService) : base(options, logService)
        {
            
        }

        public Message Execute(Message input)
        {
            try
            {
                var optLocalFile = this.CreateLocalTargetFile(input.FtpUri);
                input.OptFilePath = optLocalFile;

                var res = Cmd.Run(Constants.Tools.JpegTran, "-copy none -optimize " + input.RawFilePath + " " + optLocalFile);
                
                this.logService.Verbose("JpenTran EXEC {0}-{1}", res.ExitCode, res.Output);
                return this.HandleCmdResult(res, input);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex.ToString());
                return this.HandleFault(input, ex.ToString());
            }
        }
    }
}
