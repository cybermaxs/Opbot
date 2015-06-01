using Opbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.FtpClient;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Opbot.Tests
{
    public class FtpTests
    {
        //public const string FtpHost = "ftp://betclick.upload.llnw.net";
        public const string FtpHost = "betclick.upload.llnw.net";
        public const string FtpUser = "betclick-ht-mlemaitre";
        public const string FtpPwd = "Dnj8wtSfpDm!";

        [Fact]
        public void ListDetails()
        {
            //var ftp = new FtpService(new Options() { Host = FtpHost, User = FtpUser, Password = FtpPwd }, null);

            //var all = ftp.ListDirectoryAsync("/media/retention/frfr/betclic/sport/emails/2015/02/04").Result;
        }

        [Fact]
        public void ListDetailsByClient()
        {
            var client = new FtpClient();
            client.Credentials = new System.Net.NetworkCredential(FtpUser, FtpPwd);
            client.Host = FtpHost;
            client.Connect();
            var items = client.GetListing("/media/retention/frfr/sitepoker", FtpListOption.AllFiles | FtpListOption.Recursive);
            client.Disconnect();

        }
    }
}
