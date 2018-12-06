namespace CommandSystem.Exceptions
{
    public class CommandInputException : CommandException
    {
        public CommandInputException()
        {
        }

        public CommandInputException(string message) : base(message)
        {
        }
    }
}