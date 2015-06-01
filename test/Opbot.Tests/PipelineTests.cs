using Opbot.Core;
using Opbot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Opbot.Tests
{
    public class PipelineTests
    {
        public const string FtpHost = "betclick.upload.llnw.net";
        public const string FtpUser = "betclick-ht-mlemaitre";
        public const string FtpPwd = "Dnj8wtSfpDm!";
        [Fact]
        public void Test()
        {
            var options = new Options() { Host = FtpHost, User = FtpUser, Password = FtpPwd };
            options.Verbose = true;


            Optimizer pipe = new Optimizer(options, new LogService(options));
            var res = pipe.Scan(@"/media/retention/frfr/logo");

            Assert.Equal(true, res);

            var p = pipe.Complete().Result;
        }
    }
}
