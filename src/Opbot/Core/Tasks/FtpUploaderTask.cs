using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.FtpClient;
using System.Text;
using System.Threading.Tasks;

namespace Opbot.Core.Tasks
{
    class FtpUploadTask : IPipelineTask<Message, Message>
    {
        private readonly string ftpHost;
        private readonly string ftpUser;
        private readonly string ftpPassword;
        private readonly bool ftpSimulate;
        private readonly ILogService logService;

        public FtpUploadTask(Options options, ILogService logService)
        {
            this.logService = logService;
            this.ftpHost = options.Host;
            this.ftpUser = options.User;
            this.ftpPassword = options.Password;
            this.ftpSimulate = options.Simulate;
        }


        public Message Execute(Message input)
        {
            if (!File.Exists(input.OptFilePath))
                return input.MarkAsFault("File does not exists =>" + input.OptFilePath);

            var ftp = new FtpFileManager(this.ftpHost, this.ftpUser, this.ftpPassword);

            try
            {

                this.logService.Verbose("FTP/PUT =>{0}", input.FtpUri);
                if(!this.ftpSimulate)
                    ftp.UploadFileAsync(input.OptFilePath, input.FtpUri).Wait();

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
