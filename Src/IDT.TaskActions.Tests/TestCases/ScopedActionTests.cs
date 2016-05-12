using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IDT.TaskActions.Tests.TestCases
{
    public class ScopedActionTests
    {
        [Test]
        public void RunAction_DisposesOfContainer()
        {
            bool disposed = false;
            var container = new ExportFactory<TestResolveClass>(() => new Tuple<TestResolveClass,Action>(new TestResolveClass(),() => disposed = true));
            var sut = new ScopedAction<TestResolveClass>(container);

            sut.RunAction(CancellationToken.None);

            Assert.That(disposed, Is.True);
        }

        [Test]
        public void RunAction_RunsConstructedClassWithProvidedCancellationToken()
        {
            var resolvedClass = new TestResolveClass();
            var container = new ExportFactory<TestResolveClass>(() => new Tuple<TestResolveClass, Action>(resolvedClass, () => { }));
            var sut = new ScopedAction<TestResolveClass>(container);
            var cancellationToken = CancellationToken.None;

            sut.RunAction(cancellationToken);

            Assert.That(resolvedClass.Token, Is.EqualTo(cancellationToken));
        }

        public class TestResolveClass : ITaskAction
        {
            public CancellationToken Token { get; set; }

            public Task RunAction(CancellationToken cancellationToken)
            {
                Token = cancellationToken;
                return Task.FromResult(string.Empty);
            }
        }
    }
}
