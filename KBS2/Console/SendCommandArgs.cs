namespace KBS2.Console
{
    public class SendCommandArgs
    {
        public string Command { get; }
        public bool Handled { get; set; }

        public SendCommandArgs(string command)
        {
            Command = command;
        }
    }
}
