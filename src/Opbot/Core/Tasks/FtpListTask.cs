using Opbot.Extensions;
using Opbot.Model;
using Opbot.Services;
using Opbot.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.FtpClient;
using System.Threading.Tasks;

namespace Opbot.Core.Tasks
{
    /// <summary>
    /// Given a root url, try to get list all files.
    /// </summary>
    internal class FtpListTask :BaseFtpTask , IPipelineTask<string, Message[]>
    {
        private readonly TimeSpan since;

        public FtpListTask(Options options, ILogService logService):base(options,logService)
        {
            this.since = options.Since ?? TimeSpan.FromDays(7);
        }

        public Message[] Execute(string input)
        {
            var watcher = Stopwatch.StartNew();

            var messages = new List<Message>();
            try
            {
                var queue = new Queue<string>();
                queue.Enqueue(input);
                while (queue.Count > 0)
                {
                    var all = queue.TakeAndRemove(25);

                    foreach (var batch in all.Batch(25))
                    {
                        var tasks = new List<Task<FtpListItem[]>>();
                        foreach (var item in batch)
                        {
                            this.logService.Verbose("FTP/LIST =>{0}", item);
                            tasks.Add(FtpClient.ListingAsync(item));
                        }

                        try
                        {
                            Task.WaitAll(tasks.ToArray());
                        }
                        catch(AggregateException aex)
                        {
                            this.logService.Error("{0} : {1} =>{2}", aex.GetType(), aex.Message, aex.ToString());
                        }

                        //parse results
                        foreach (var t in tasks)
                        {
                            if (t.IsFaulted || t.IsCanceled)
                                continue;

                            var listItems = t.Result;
                            foreach (var listItem in listItems)
                            {
                                switch (listItem.Type)
                                {
                                    case FtpFileSystemObjectType.Directory:
                                        if (Constants.ExcludedPath.Intersect(listItem.FullName.Split(new char[] {'/'},  StringSplitOptions.RemoveEmptyEntries)).Count()>0)
                                            continue;
                                        queue.Enqueue(listItem.FullName);
                                        break;
                                    case FtpFileSystemObjectType.File:
                                        if (listItem.FullName.EndsWith(Constants.FileType.Jpeg)
                                            || listItem.FullName.EndsWith(Constants.FileType.Png)) //is File ?
                                        {
                                            if (listItem.Modified.Add(this.since) > DateTime.UtcNow)
                                            {
                                                var msg = new Message();
                                                msg.FtpUri = listItem.FullName;
                                                msg.LastModified = listItem.Modified;
                                                messages.Add(msg);
                                            }
                                        }

                                        break;
                                    default:
                                        continue;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                this.logService.Error("{0} : {1} =>{2}", ex.GetType(), ex.Message, ex.ToString());
                messages.Clear();
            }

            watcher.Stop();
            this.logService.Info("{0} files done in {1} ms", messages.Count, watcher.ElapsedMilliseconds);


            return messages.ToArray();
        }



    }
}
