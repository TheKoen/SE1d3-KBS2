namespace KBS2.Exceptions
{
    public class InvalidParametersException : CommandException
    {
        public InvalidParametersException() : base("Invalid parameters")
        {
        }

        public InvalidParametersException(string message) : base(message)
        {
        }
    }
}