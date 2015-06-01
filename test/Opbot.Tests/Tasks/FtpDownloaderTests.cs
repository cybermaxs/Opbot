using Opbot.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Opbot.Tests.Tasks
{
    public class FtpDownlaoderTests
    {
        public const string FtpHost = "betclick.upload.llnw.net";
        public const string FtpUser = "betclick-ht-mlemaitre";
        public const string FtpPwd = "Dnj8wtSfpDm!";

        [Fact]
        public void Test()
        {
            //var ftp = new FtpDownloadTask(new Options() { Host = FtpHost, User = FtpUser, Password = FtpPwd }, null);

            //var all = ftp.Execute("/media/retention/frfr/logo/moneyclic_logo.jpg");
        }
    }
}
