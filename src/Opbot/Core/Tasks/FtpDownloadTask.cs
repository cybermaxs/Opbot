using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System;
using System.IO;

namespace Opbot.Core.Tasks
{
    class FtpDownloadTask : IPipelineTask<Message, Message>
    {
        private readonly string ftpHost;
        private readonly string ftpUser;
        private readonly string ftpPassword;
        private readonly string baseFolder;
        private readonly ILogService logService;

        public FtpDownloadTask(Options options, ILogService logService)
        {
            this.logService = logService;
            this.ftpHost = options.Host;
            this.ftpUser = options.User;
            this.baseFolder = options.WorkingFolder;
            this.ftpPassword = options.Password;
        }


        public Message Execute(Message input)
        {
            var ftp = new FtpFileManager(this.ftpHost, this.ftpUser, this.ftpPassword);

            try
            {              
                var localFile = Path.Combine(this.baseFolder, "In") + input.FtpUri.Replace('/', '\\');
                var fulldir = Path.GetDirectoryName(localFile);
                if (!Directory.Exists(fulldir))
                    Directory.CreateDirectory(fulldir);

                this.logService.Verbose("FTP/GET =>{0}", input.FtpUri);
                ftp.DownloadFileAsync(input.FtpUri, localFile).Wait();
                input.RawFilePath = localFile;

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
