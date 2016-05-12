using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;

namespace IDT.TaskActions.Tests.TestCases
{
    public class HandleExceptionAndRetryActionDecoratorTests
    {
        [Test]
        public async Task RunAction_WhenNoException_RunsAction()
        {
            var action = Substitute.For<ITaskAction>();
            var handler = Substitute.For<IExceptionHandler>();
            var cancellationToken = CancellationToken.None;

            var sut = new NeverRetryActionDecorator(action, handler);

            await sut.RunAction(cancellationToken);

            action.Received().RunAction(cancellationToken).IgnoreAwait();
        }

        [Test]
        public async Task RunAction_WhenNoException_DoesNotRunExceptionHandler()
        {
            var action = Substitute.For<ITaskAction>();
            var handler = Substitute.For<IExceptionHandler>();
            var cancellationToken = CancellationToken.None;

            var sut = new NeverRetryActionDecorator(action, handler);

            await sut.RunAction(cancellationToken);

            handler.DidNotReceiveWithAnyArgs().HandleException(null);
        }

        [Test]
        public async Task RunAction_WhenException_RunsExceptionHandler()
        {
            var exception = new Exception(string.Empty);
            var action = Substitute.For<ITaskAction>();
            action.When(x => x.RunAction(Arg.Any<CancellationToken>()))
                .Do(c => { throw exception; });
            var handler = Substitute.For<IExceptionHandler>();
            var cancellationToken = CancellationToken.None;

            var sut = new NeverRetryActionDecorator(action, handler);

            await sut.RunAction(cancellationToken);

            handler.Received().HandleException(exception);
        }

        [Test]
        public async Task RunAction_WhenException_WillRetry()
        {
            var exception = new Exception(string.Empty);
            var action = Substitute.For<ITaskAction>();
            action.When(x => x.RunAction(Arg.Any<CancellationToken>()))
                .Do(c => { throw exception; });
            var handler = Substitute.For<IExceptionHandler>();
            var interval = Substitute.For<IInterval>();
            interval.Calculate(Arg.Any<int>()).Returns(TimeSpan.FromMilliseconds(1));
            var cancellationToken = CancellationToken.None;

            var sut = new SingleRetryActionDecorator(action, handler){ RetryInterval = interval };

            await sut.RunAction(cancellationToken);

            action.Received(2).RunAction(cancellationToken).IgnoreAwait();
        }

        [Test]
        public async Task RunAction_WhenException_CallsIntervalWithNumberOfExceptions()
        {
            var exception = new Exception(string.Empty);
            var action = Substitute.For<ITaskAction>();
            action.When(x => x.RunAction(Arg.Any<CancellationToken>()))
                .Do(c => { throw exception; });
            var handler = Substitute.For<IExceptionHandler>();
            var interval = Substitute.For<IInterval>();
            interval.Calculate(Arg.Any<int>()).Returns(TimeSpan.FromMilliseconds(1));
            var cancellationToken = CancellationToken.None;

            var sut = new SingleRetryActionDecorator(action, handler) { RetryInterval = interval };

            await sut.RunAction(cancellationToken);

            interval.Received(1).Calculate(1);
        }

        [Test]
        public void RunAction_WhenCanceledWhileInRetryWait_ThrowsOperationCanceledException()
        {
            var action = Substitute.For<ITaskAction>();
            var handler = Substitute.For<IExceptionHandler>();
            var interval = Substitute.For<IInterval>();
            interval.Calculate(Arg.Any<int>()).Returns(TimeSpan.FromDays(1));

            var sut = new SingleRetryActionDecorator(action, handler) { RetryInterval = interval };

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.Catch<OperationCanceledException>(async () => await sut.RunAction(cts.Token));
            }
        }

        [Test]
        public void RunAction_WhenActionThrowsOperationCanceledException_ThrowsOperationCanceledException()
        {
            var exception = new OperationCanceledException();
            var action = Substitute.For<ITaskAction>();
            action.When(x => x.RunAction(Arg.Any<CancellationToken>()))
                .Do(c => { throw exception; });
            var handler = Substitute.For<IExceptionHandler>();
            var cancellationToken = CancellationToken.None;

            var sut = new NeverRetryActionDecorator(action, handler);

            Assert.Catch<OperationCanceledException>(async () => await sut.RunAction(cancellationToken));
        }

        [Test]
        public async Task RunAction_WhenActionThrowsOperationCanceledException_DoesNotRunExceptionHandler()
        {
            var exception = new OperationCanceledException();
            var action = Substitute.For<ITaskAction>();
            action.When(x => x.RunAction(Arg.Any<CancellationToken>()))
                .Do(c => { throw exception; });
            var handler = Substitute.For<IExceptionHandler>();
            var cancellationToken = CancellationToken.None;

            var sut = new NeverRetryActionDecorator(action, handler);

            try
            {
                await sut.RunAction(cancellationToken);
            }
            catch {}
            
            handler.DidNotReceiveWithAnyArgs().HandleException(null);
        }


        public class NeverRetryActionDecorator : RetryOnExceptionActionDecorator
        {
            public NeverRetryActionDecorator(ITaskAction action, IExceptionHandler handler)
                :base(action,handler)
            {
            }

            protected override bool ShouldRetryAfterNExceptions(int consecutiveExceptions)
            {
                return false;
            }
        }

        public class SingleRetryActionDecorator : RetryOnExceptionActionDecorator
        {
            public SingleRetryActionDecorator(ITaskAction action, IExceptionHandler handler)
                : base(action, handler)
            {
            }

            protected override bool ShouldRetryAfterNExceptions(int consecutiveExceptions)
            {
                return consecutiveExceptions <= 1;
            }
        }   
    }

    public static class Helper
    {
        public static void IgnoreAwait(this Task task)
        {
            //This is just here to avoid getting a compiler warning.
        }
    }
}
