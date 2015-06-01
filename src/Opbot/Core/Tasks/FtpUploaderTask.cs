using Opbot.Model;
using Opbot.Services;
using System;
using System.IO;

namespace Opbot.Core.Tasks
{
    class FtpUploadTask : BaseFtpTask, IPipelineTask<Message, Message>
    {
        private readonly bool doUpload;

        public FtpUploadTask(Options options, ILogService logService)
            : base(options, logService)
        {
            this.doUpload = !options.Simulate;
        }


        public Message Execute(Message input)
        {
            if (!File.Exists(input.OptFilePath))
                return input.MarkAsFault("File does not exists =>" + input.OptFilePath);

            try
            {
                if (this.doUpload)
                {
                    this.logService.Verbose("FTP/PUT =>{0}", input.FtpUri);
                    FtpClient.UploadFileAsync(input.OptFilePath, input.FtpUri).Wait();
                }

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
