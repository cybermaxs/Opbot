using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System.IO;

namespace Opbot.Core.Tasks
{
    class BaseFtpTask
    {
        private readonly string baseFolder;
        protected readonly ILogService logService;

        public BaseFtpTask(Options options, ILogService logService)
        {
            this.logService = logService;
            this.baseFolder = options.WorkingFolder;
            this.FtpClient = new FtpFileManager(options.Host, options.User, options.Password);
        }

        protected FtpFileManager FtpClient { get; private set; }

        protected string CreateLocalSourceFile(string originUri)
        {
            string localPath = Path.Combine(this.baseFolder, "In") + originUri.Replace('/', '\\');
            var fulldir = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(fulldir))
                Directory.CreateDirectory(fulldir);
            
            return localPath;
        }

        protected Message HandleFault(Message msg, string faultReason)
        {
            msg.MarkAsFault(faultReason);
            if (File.Exists(msg.RawFilePath))
                File.Delete(msg.RawFilePath);
            msg.RawFilePath = string.Empty;
            return msg;
        }
    }
}
