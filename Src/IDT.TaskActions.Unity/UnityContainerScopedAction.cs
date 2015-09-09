using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System;
using System.Threading;

namespace IDT.TaskActions.Unity
{
    public class UnityContainerScopedAction<TAction> : ITaskAction
        where TAction : ITaskAction
    {
        private readonly Func<IUnityContainer> _containerFactory;

        public UnityContainerScopedAction(Func<IUnityContainer> containerFactory)
        {
            _containerFactory = containerFactory;
        }

        public async Task RunAction(CancellationToken cancellationToken)
        {
            using (var container = _containerFactory())
            {
                await container.Resolve<TAction>().RunAction(cancellationToken);
            }
        }
    }
}
