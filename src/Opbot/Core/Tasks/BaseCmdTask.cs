using Opbot.Model;
using Opbot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opbot.Core.Tasks
{
    class BaseCmdTask
    {
        protected readonly ILogService logService;
        private readonly string baseFolder;

        public BaseCmdTask(Options options, ILogService logService)
        {
            this.logService = logService;
            this.baseFolder = options.WorkingFolder;
        }

        protected string CreateLocalTargetFile(string originUri, bool createIt = true)
        {
            string localPath = Path.Combine(this.baseFolder, "Out") + originUri.Replace('/', '\\');
            var fulldir = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(fulldir))
                Directory.CreateDirectory(fulldir);

            if (createIt)
                using (File.Create(localPath)) { };

            return localPath;
        }

        protected Message HandleCmdResult(Opbot.Utils.Cmd.CommandResult result, Message msg)
        {
            if (result.ExitCode != 0)
            {
                this.HandleFault(msg, result.Output);
                return msg;
            }
            return msg;
        }

        protected Message HandleFault(Message msg, string faultReason)
        {
            msg.MarkAsFault(faultReason);
            if (File.Exists(msg.OptFilePath))
                File.Delete(msg.OptFilePath);
            msg.OptFilePath = string.Empty;
            return msg;
        }
    }
}
