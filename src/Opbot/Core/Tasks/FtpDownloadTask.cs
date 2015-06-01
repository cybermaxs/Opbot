using Opbot.Model;
using Opbot.Services;
using System;
using System.IO;

namespace Opbot.Core.Tasks
{
    class FtpDownloadTask :BaseFtpTask, IPipelineTask<Message, Message>
    {
        public FtpDownloadTask(Options options, ILogService logService) : base(options, logService)
        {

        }


        public Message Execute(Message input)
        {
            try
            {
                var localFile = this.CreateLocalSourceFile(input.FtpUri);

                this.logService.Verbose("FTP/GET =>{0}", input.FtpUri);
                FtpClient.DownloadFileAsync(input.FtpUri, localFile).Wait();

                if(File.Exists(localFile))
                    input.RawFilePath = localFile;

                return input;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex.ToString());
                return this.HandleFault(input, ex.ToString());
            }
        }
    }
}
