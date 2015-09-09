using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace IDT.TaskActions.Tests.TestCases
{
    public class ParallelizeActionDecoratorTests
    {
        [Test]
        public async Task RunAction_RunsAllActions()
        {
            //Arrange
            int invokedActionCount = 0;

            var fixture = new TaskActionFixture();
            var actions = fixture.Freeze<IEnumerable<ITaskAction>>().ToList();
            actions.ForEach(x => x.When(y => y.RunAction(Arg.Any<CancellationToken>()))
                                  .Do(y => invokedActionCount++));

            var sut = fixture.Create<ParallelizeActionDecorator>();

            //Act
            await sut.RunAction(CancellationToken.None);

            //Assert
            Assert.That(invokedActionCount, Is.EqualTo(actions.Count));
        }

        [Test]
        public void RunAction_RunsAllActionsInParallel()
        {
            //Arrange
            int totalActionCount = 0;
            int invokedActionCount = 0;
            var cancellationTokenSource = new CancellationTokenSource();

            Action stopWhenAllAreRunning = () =>
                                               {
                                                   invokedActionCount++;
                                                   if (invokedActionCount == totalActionCount)
                                                   {
                                                       cancellationTokenSource.Cancel();
                                                   }
                                               };

            var fixture = new TaskActionFixture();
            var timeLimitActions = fixture.CreateMany<MockTimeLimitAction>().ToList();
            timeLimitActions.ForEach(x => x.OnStarted = stopWhenAllAreRunning);
            totalActionCount = timeLimitActions.Count;
            fixture.Register<IEnumerable<ITaskAction>>(() => timeLimitActions);
            var sut = fixture.Create<ParallelizeActionDecorator>();

            //Act & Assert
            Assert.Catch<OperationCanceledException>(async () => await sut.RunAction(cancellationTokenSource.Token));
        }

        [Test]
        public void RunAction_WhenOneActionCompletes_CancelsOtherActionAndCompletes()
        {
            //Arrange
            var cancellationTokenSource = new CancellationTokenSource();

            var fixture = new TaskActionFixture();
            var terminatingAction = fixture.Create<ITaskAction>();
            var infiniteAction = fixture.Create<MockTimeLimitAction>();
            fixture.Register<IEnumerable<ITaskAction>>(() => new[] {terminatingAction, infiniteAction});
            var sut = fixture.Create<ParallelizeActionDecorator>();

            //Act & Assert
            Assert.DoesNotThrow(async () => await sut.RunAction(cancellationTokenSource.Token));
        }

        [Test]
        public async Task RunAction_WhenAlreadyCancelled_DoesNotStart()
        {
            //Arrange
            bool actionInvoked = false;
            var cancellationTokenSource = new CancellationTokenSource();

            var fixture = new TaskActionFixture();
            var errorAction = fixture.Create<ITaskAction>();
            errorAction.When(y => y.RunAction(Arg.Any<CancellationToken>()))
                       .Do(y => actionInvoked = true);
            fixture.Register<IEnumerable<ITaskAction>>(() => new[] {errorAction});
            var sut = fixture.Create<ParallelizeActionDecorator>();

            cancellationTokenSource.Cancel();

            //Act
            try
            {
                await sut.RunAction(cancellationTokenSource.Token);
            }
            catch (Exception)
            {
            }

            //Assert
            Assert.That(actionInvoked, Is.False);
        }

        [Test]
        public async Task RunAction_OneListenerFails_CancelsOtherListeners()
        {
            //Arrange
            var fixture = new TaskActionFixture();
            fixture.Register(() => TimeSpan.FromSeconds(1.0));
            var errorAction = fixture.Create<MockExceptionAction>();
            var infiniteAction = fixture.Create<MockTimeLimitAction>();
            fixture.Register<IEnumerable<ITaskAction>>(() => new ITaskAction[] {errorAction, infiniteAction});
            var sut = fixture.Create<ParallelizeActionDecorator>();

            //Act & Assert
            try
            {
                await sut.RunAction(CancellationToken.None);
            }
            catch (ApplicationException)
            {
            }
        }

        [Test]
        public void RunAction_WhenListenerFails_ThrowsListenerException()
        {
            //Arrange
            var fixture = new TaskActionFixture();
            fixture.Register(() => TimeSpan.FromSeconds(1.0));
            var errorAction = fixture.Create<MockExceptionAction>();
            var infiniteAction = fixture.Create<MockTimeLimitAction>();
            fixture.Register(() => new ITaskAction[] {errorAction, infiniteAction}.AsEnumerable());
            var sut = fixture.Create<ParallelizeActionDecorator>();

            //Act & Assert
            Assert.Catch<ApplicationException>(async () => await sut.RunAction(CancellationToken.None));
        }
    }
}
