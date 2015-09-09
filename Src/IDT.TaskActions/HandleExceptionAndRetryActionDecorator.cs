using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public abstract class HandleExceptionAndRetryActionDecorator : ITaskAction
    {
        private readonly ITaskAction _action;
        private readonly IExceptionHandler _exceptionHandler;

        protected HandleExceptionAndRetryActionDecorator(ITaskAction action, IExceptionHandler exceptionHandler)
        {
            RetryInterval = BackoffInterval.Default;

            _action = action;
            _exceptionHandler = exceptionHandler;
        }

        public IInterval RetryInterval { get; set; }
        
        public async Task RunAction(CancellationToken cancellationToken)
        {
            int numberOfConsecutiveExceptions = 0;
            bool retry = false;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await _action.RunAction(cancellationToken);
                    numberOfConsecutiveExceptions = 0;
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        throw;
                    }
                    numberOfConsecutiveExceptions++;
                    _exceptionHandler.HandleException(ex);
                }

                retry = ShouldRetryAfterNExceptions(numberOfConsecutiveExceptions);
                if (retry)
                {
                    var resetInterval = this.RetryInterval.Calculate(numberOfConsecutiveExceptions);
                    await Task.Delay(resetInterval,cancellationToken);
                }
            } while (retry);
        }

        protected abstract bool ShouldRetryAfterNExceptions(int consecutiveExceptions);
    }
}