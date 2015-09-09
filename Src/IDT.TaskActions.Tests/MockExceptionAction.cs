using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions.Tests
{
    class MockExceptionAction : ITaskAction
    {
        public Task RunAction(CancellationToken cancellationToken)
        {
            var source = new TaskCompletionSource<bool>();
            source.SetException(new ApplicationException("Exception Thrown"));
            return source.Task;
        }
    }
}
