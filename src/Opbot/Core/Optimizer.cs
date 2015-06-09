using Opbot.Core.Tasks;
using Opbot.Model;
using Opbot.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Opbot.Core
{
    internal class Optimizer
    {
        private readonly ILogService logService;
        private readonly Options options;
        const int MaxParallelism = 15;

        public Optimizer(Options options, ILogService logService)
        {
            this.logService = logService;
            this.options = options;

            this.CreatePipeline();

            this.AllResults = new List<Message>();
            this.OptimalResults = new List<Message>();
        }

        public ITargetBlock<string> InputBlock { get; set; }
        public IDataflowBlock OutputBlock { get; set; }

        public List<Message> AllResults { get; private set; }
        public List<Message> OptimalResults { get; private set; }

        private void CreatePipeline()
        {
            //common faulted logger
            var faultedTask = new ActionBlock<Message>(item =>
            {
                this.AllResults.Add(item);
                this.logService.Warning("file '{0}' is not eligible. Reason : {1}", item.FtpUri, item.FaultReason);
            });

            // get all files
            var ftpListTask = new TransformManyBlock<string, Message>(root =>
            {
                var t = new FtpListTask(this.options, logService);
                return t.Execute(root);
            });
            this.InputBlock = ftpListTask;

            //download candidates
            var ftpDownloadTask = new TransformBlock<Message, Message>(file =>
            {
                var t = new FtpDownloadTask(this.options, logService);
                return t.Execute(file);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = MaxParallelism });

            //png optim
            var optimizePngTask = new TransformBlock<Message, Message>(item =>
            {
                var t = new PngTask(this.options, logService);
                return t.Execute(item);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = MaxParallelism * 4 });
            //jpeg optim
            var optimizeJpegTask = new TransformBlock<Message, Message>(item =>
            {
                var t = new JpegTask(this.options, logService);
                return t.Execute(item);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = MaxParallelism * 4 });
            //stats
            var statsTask = new TransformBlock<Message, Message>(item =>
            {
                var t = new FileStatsTask(logService);
                return t.Execute(item);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = MaxParallelism * 4 });

            var electionTask = new TransformManyBlock<Message, Message>(item =>
            {
                this.AllResults.Add(item);

                if (item.RawFileSize == 0 || item.OptFileSize == 0)
                    this.logService.Warning("'{0}' problem with {1}=>{2}", item.RawFilePath, item.RawFileSize, item.OptFileSize);

                if (item.OptFileSize > item.RawFileSize)
                    this.logService.Warning("'{0}' is not optimal with {1}=>{2}", item.RawFilePath, item.RawFileSize, item.OptFileSize);


                if (item.OptFileSize < item.RawFileSize)
                {
                    this.logService.Info("'{0}' is a good candidate {1}=>{2}", item.FtpUri, item.RawFileSize, item.OptFileSize);

                    long delta = Math.Abs(item.RawFileSize - item.OptFileSize);
                    var pct = Math.Round((double)delta / item.RawFileSize * 100, 0);

                    if (delta > 10 * 1014 || pct > 10D)
                    {
                        this.OptimalResults.Add(item);
                        return new Message[] { item };
                    }

                }

                return new Message[0];
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = MaxParallelism * 4 });

            var ftpUploadTask = new ActionBlock<Message>(item =>
            {
                var t = new FtpUploadTask(this.options, logService);
                t.Execute(item);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = MaxParallelism });


            this.OutputBlock = ftpUploadTask;

            //pipeline
            ftpListTask.LinkTo(ftpDownloadTask, new DataflowLinkOptions() { PropagateCompletion = true }, m => { return !m.IsFaulted; });
            ftpListTask.LinkTo(faultedTask, m => { return m.IsFaulted; });

            ftpDownloadTask.LinkTo(optimizeJpegTask, new DataflowLinkOptions() { PropagateCompletion = true }, m => { return m.FtpUri.Contains(".jpg"); });
            ftpDownloadTask.LinkTo(optimizePngTask, new DataflowLinkOptions() { PropagateCompletion = true }, m => { return m.FtpUri.Contains(".png"); });
            ftpDownloadTask.LinkTo(faultedTask, m => { return m.IsFaulted; });

            optimizeJpegTask.LinkTo(statsTask, m => { return !m.IsFaulted; });
            optimizeJpegTask.LinkTo(faultedTask, m => { return m.IsFaulted; });
            optimizePngTask.LinkTo(statsTask, m => { return !m.IsFaulted; });
            optimizePngTask.LinkTo(faultedTask, m => { return m.IsFaulted; });
            Task.WhenAll(optimizeJpegTask.Completion, optimizePngTask.Completion).ContinueWith(_ => statsTask.Complete());

            statsTask.LinkTo(electionTask, new DataflowLinkOptions() { PropagateCompletion = true }, m => { return !m.IsFaulted; });
            statsTask.LinkTo(faultedTask, m => { return m.IsFaulted; });

            electionTask.LinkTo(ftpUploadTask, new DataflowLinkOptions() { PropagateCompletion = true }, m => { return !m.IsFaulted; });
            electionTask.LinkTo(faultedTask, m => { return m.IsFaulted; });
        }

        public bool Scan(string input)
        {
            return this.InputBlock.Post(input);

        }

        public async Task<List<Message>> Complete()
        {
            this.InputBlock.Complete();
            await OutputBlock.Completion;

            return this.AllResults;
        }
    }
}
