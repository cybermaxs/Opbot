using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opbot.Model
{
    class Message
    {
        public bool IsFaulted { get; set; }
        public string FaultReason { get; set; }
        public DateTime LastModified { get; set; }
        public string FtpUri { get; set; }
        public string RawFilePath { get; set; }
        public long RawFileSize { get; set; }
        public string OptFilePath { get; set; }
        public long OptFileSize { get; set; }

        public Message MarkAsFault(string reason)
        {
            this.IsFaulted = true;
            this.FaultReason = reason;
            return this;
        }
    }
}
