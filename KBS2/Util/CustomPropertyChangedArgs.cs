namespace KBS2.Util
{
    public class CustomPropertyChangedArgs
    {
        public dynamic ValueBefore { get; }
        public dynamic ValueAfter { get; }

        public CustomPropertyChangedArgs(dynamic valueBefore, dynamic valueAfter)
        {
            ValueBefore = valueBefore;
            ValueAfter = valueAfter;
        }
    }
}