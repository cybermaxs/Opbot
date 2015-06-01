using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System;

namespace Opbot.Core.Tasks
{
    class PngTask : BaseCmdTask,  IPipelineTask<Message, Message>
    {
        public PngTask(Options options, ILogService logService):base(options, logService)
        {
            
        }

        public Message Execute(Message input)
        {
            try
            {
                var optLocalFile = this.CreateLocalTargetFile(input.FtpUri);
                input.OptFilePath = optLocalFile;

                var res = Cmd.Run(Constants.Tools.OptiPNG, "-strip all -clobber -force -fix -out " + optLocalFile + " " + input.RawFilePath);

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
