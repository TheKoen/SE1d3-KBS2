namespace KBS2.Exceptions
{
    public class EmptyCommandException : CommandException
    {
        public EmptyCommandException() : this("Empty input")
        {
        }

        public EmptyCommandException(string message) : base(message)
        {
        }
    }
}