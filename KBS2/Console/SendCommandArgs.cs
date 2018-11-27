namespace KBS2.Console
{
    public class SendCommandArgs
    {
        public string Command { get; }
        public bool Handled { get; set; } // TODO: I made this property for you guys, why are you not using it??

        public SendCommandArgs(string command)
        {
            Command = command;
        }
    }
}
