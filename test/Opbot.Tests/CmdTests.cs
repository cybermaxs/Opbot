using Opbot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Opbot.Tests
{
    public class CmdTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Cmd_WhenRun_ShouldEcho()
        {
            var result = Cmd.Run(@"c:\windows\system32\cmdkey.exe");

            Assert.Equal(0, result.ExitCode);
            Assert.Equal("test", string.Join("",result.Output));
        }
    }
}
