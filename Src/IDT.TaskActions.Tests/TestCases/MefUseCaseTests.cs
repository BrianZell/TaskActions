using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Threading;

namespace IDT.TaskActions.Tests.TestCases
{
    class MefUseCaseTests
    {
        [Test]
        public async Task TestName()
        {
            var rb = new RegistrationBuilder();
            rb.ForType<ITaskAction>()
              .Export<TestTask>();
            rb.ForType<ParallelizeActionDecorator>()
              .Export<ParallelizeActionDecorator>();
            var tc = new TypeCatalog(new [] {typeof(ITaskAction),typeof(ParallelizeActionDecorator)},rb);

            var cc = new CompositionContainer(tc);
            var x = cc.GetExportedValue<ParallelizeActionDecorator>();
            await x.RunAction(CancellationToken.None);
        }
    }

    class TestTask : ITaskAction
    {
        public Task RunAction(CancellationToken cancellationToken)
        {
            Console.WriteLine("HI!");
            return Task.FromResult(string.Empty);
        }
    }
}
