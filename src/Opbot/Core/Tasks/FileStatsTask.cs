using Opbot.Model;
using Opbot.Services;
using System.IO;

namespace Opbot.Core.Tasks
{
    class FileStatsTask : IPipelineTask<Message, Message>
    {
        private readonly ILogService logService;

        public FileStatsTask(ILogService logService)
        {
            this.logService = logService;

        }
        public Message Execute(Message input)
        {
            if (!File.Exists(input.OptFilePath) || !File.Exists(input.OptFilePath))
            {
                input.IsFaulted = true;
                input.FaultReason = "file does not exists on disk";
                return input;
            }

            var rawinfo = new FileInfo(input.RawFilePath);
            input.RawFileSize = rawinfo.Length;

            var optinfo = new FileInfo(input.OptFilePath);
            input.OptFileSize = optinfo.Length;

            return input;
        }
    }
}
