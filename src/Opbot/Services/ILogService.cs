using Serilog;
using System;
namespace Opbot.Services
{
    interface ILogService
    {
        void Verbose(string pattern, params object[] parameters);
        void Info(string pattern, params object[] parameters);
        void Warning(string pattern, params object[] parameters);
        void Error(string pattern, params object[] parameters);
    }
}
