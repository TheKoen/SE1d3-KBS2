namespace CommandSystem.PropertyManagement
{
    public sealed class UserPropertyChangedArgs
    {
        public dynamic ValueBefore { get; }
        public dynamic ValueAfter { get; }

        public UserPropertyChangedArgs(dynamic valueBefore, dynamic valueAfter)
        {
            ValueBefore = valueBefore;
            ValueAfter = valueAfter;
        }
    }
}