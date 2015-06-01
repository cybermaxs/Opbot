using CommandLine;
using Opbot.Core;
using Opbot.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Opbot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(";", args));
            var options = new Options();


            if (Parser.Default.ParseArguments(args, options))
            {
                //post apply settings
                options.Since = options.Since ?? TimeSpan.FromDays(30);
                options.WorkingFolder = string.IsNullOrEmpty(options.WorkingFolder) ? Path.Combine(Environment.CurrentDirectory, DateTime.UtcNow.Ticks.ToString()) : options.WorkingFolder;

                var logger = new LogService(options);
                logger.Info("Started ...");

                ServicePointManager.DefaultConnectionLimit = 128;
                int cpuw = 0, iow = 0;
                ThreadPool.GetMinThreads(out cpuw, out iow);
                ThreadPool.SetMinThreads(Math.Min(128, cpuw), Math.Min(128, iow));

                Optimizer pipeline = new Optimizer(options, logger);
                pipeline.Scan(options.RemoteFolder);

                pipeline.Complete().Wait();
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit ...");
                Console.ReadKey();
            }
        }
    }
}
