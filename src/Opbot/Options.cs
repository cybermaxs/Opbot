using CommandLine;
using System;
using System.Text;

namespace Opbot
{
    public class Options
    {
        [Option('h', "host")]
        public string Host { get; set; }

        [Option('u', "user",  DefaultValue="")]
        public string User { get; set; }

        [Option('p', "password", DefaultValue = "")]
        public string Password { get; set; }

        [Option('s', "since", DefaultValue = null)]
        public TimeSpan? Since { get; set; }

        [Option('v', "verbose", DefaultValue = false)]
        public bool Verbose { get; set; }

        [Option('s', "simulate", DefaultValue = false)]
        public bool Simulate { get; set; }

        [Option('w', "working folder")]
        public string WorkingFolder { get; set; }

        [Option('r', "Remote starting folder", DefaultValue="/")]
        public string RemoteFolder { get; set; }

  

    }
}
