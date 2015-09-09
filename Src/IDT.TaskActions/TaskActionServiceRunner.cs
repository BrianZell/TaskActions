using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class TaskActionServiceRunner : IDisposable
    {
        public TaskActionServiceRunner(ITaskAction action, IExceptionHandler exceptionHandler)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _retryUntilCanceledActionDecorator = new RetryUntilCanceledActionDecorator(action, exceptionHandler);
        }

        public IInterval ResetInterval
        {
            get { return _retryUntilCanceledActionDecorator.RetryInterval; }
            set { _retryUntilCanceledActionDecorator.RetryInterval = value; }
        }

        public void Start()
        {
            _task = _retryUntilCanceledActionDecorator.RunAction(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            if (_task == null || _task.IsCompleted)
            {
                return;
            }

            try
            {
                _cancellationTokenSource.Cancel();
                _task.Wait();
            }
            catch (AggregateException exception)
            {
                //OperationCancelException is expected, since we cancel to stop the service.
                exception.Handle(ex => ex is OperationCanceledException);
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _task.Dispose();
                _task = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly RetryUntilCanceledActionDecorator _retryUntilCanceledActionDecorator;
        private Task _task;
    }
}