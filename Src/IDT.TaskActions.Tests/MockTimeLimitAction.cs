using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions.Tests
{
    class MockTimeLimitAction : ITaskAction
    {
        private readonly TimeSpan _cancelAfterTimeSpan;
        public Action OnStarted = () => { };

        public MockTimeLimitAction(TimeSpan cancelAfterTimeSpan)
        {
            _cancelAfterTimeSpan = cancelAfterTimeSpan;
        }

        public async Task RunAction(CancellationToken cancellationToken)
        {
            OnStarted();
            await Task.Delay(_cancelAfterTimeSpan, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("TimeSpan expired without the MockTimeLimitAction being canceled.");
            }
        }
    }
}
