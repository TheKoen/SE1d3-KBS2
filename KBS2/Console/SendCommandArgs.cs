using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Console
{
    public class SendCommandArgs
    {
        public string Command { get; }

        public SendCommandArgs(string command)
        {
            Command = command;
        }
    }
}
