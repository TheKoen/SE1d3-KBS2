namespace KBS2.Exceptions
{
    public class UnknownCommandException : CommandException
    {
        public UnknownCommandException()
        {
        }

        public UnknownCommandException(string message) : base(message)
        {
        }
    }
}