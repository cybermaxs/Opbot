using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opbot.Core.Tasks
{
    class PngTask : IPipelineTask<Message, Message>
    {
        private readonly ILogService logService;
        private readonly string baseFolder;

        public PngTask(Options options, ILogService logService)
        {
            this.logService = logService;
            this.baseFolder = options.WorkingFolder;
        }

        public Message Execute(Message input)
        {
            try
            {
                var optLocalFile = Path.Combine(this.baseFolder, "Out") + input.FtpUri.Replace('/', '\\');
                var fulldir = Path.GetDirectoryName(optLocalFile);
                if (!Directory.Exists(fulldir))
                    Directory.CreateDirectory(fulldir);
                using (File.Create(optLocalFile)) ;

                input.OptFilePath = optLocalFile;
                var res = Cmd.Run(Constants.Tools.OptiPNG, "-strip all -clobber -force -fix -out " + optLocalFile + " " + input.RawFilePath);
                this.logService.Verbose("OptiPNG EXEC {0}-{1}", res.ExitCode, res.Output);
                return input;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex.ToString());
                return input.MarkAsFault(ex.Message);
            }
        }
    }
}
