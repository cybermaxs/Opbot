using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opbot.Utils
{
    public class Cmd
    {
        public class CommandResult
        {
            public int ExitCode { get; set; }
            public string Output { get; set; }
            public string Error { get; set; }
        }
        /// <summary>
        /// http://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static CommandResult Run(string filename, string arguments = null, int timeoutInMs = 30000)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            var stdOutput = new StringBuilder();
            var stdError = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => stdError.AppendLine(args.Data);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit(timeoutInMs))
                    process.Kill();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            return new CommandResult() { ExitCode = process.ExitCode, Output = stdOutput.ToString(), Error=stdError.ToString() };

            //if (process.ExitCode == 0)
            //{
            //    return new CommandResult() { ExitCode = process.ExitCode, Output = stdOutput.ToString() };
            //}
            //else
            //{
                //var message = new StringBuilder();

                //if (!string.IsNullOrEmpty(stdError))
                //{
                //    message.AppendLine(stdError);
                //}

                //if (stdOutput.Length != 0)
                //{
                //    message.AppendLine("Std output:");
                //    message.AppendLine(stdOutput.ToString());
                //}

                //throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            //}
        }

        private static string Format(string filename, string arguments)
        {
            return "'" + filename + ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) + "'";
        }
    }
}
