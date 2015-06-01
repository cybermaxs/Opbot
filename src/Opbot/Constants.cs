using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opbot
{
    class Constants
    {
        public static string[] ExcludedPath = new string[] { "bin", "dev", "etc", "lib", "lib64", "proc", "usr", "var" };
        public static string[] SupportedExtensions = new string[] { ".jpg", ".png" };

        public class FileType
        {
            public const string Png = ".png";
            public const string Jpeg = ".jpg";
        }

        public class Tools
        {
            public const string OptiPNG = @"Tools\optipng.exe";
            public const string JpegTran = @"Tools\jpegtran.exe";
            public const string GitSicle = @"Tools\gifsicle.exe";
        }
    }
}
