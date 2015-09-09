namespace IDT.TaskActions
{
    public class RetryOnExceptionActionDecorator : HandleExceptionAndRetryActionDecorator
    {
        public RetryOnExceptionActionDecorator(ITaskAction action, IExceptionHandler handler)
            : base(action, handler)
        {
        }

        protected override bool ShouldRetryAfterNExceptions(int consecutiveExceptions)
        {
            return consecutiveExceptions > 0;
        }
    }
}
