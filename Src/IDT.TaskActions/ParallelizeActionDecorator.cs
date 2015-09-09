using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class ParallelizeActionDecorator : ITaskAction
    {
        private readonly IEnumerable<ITaskAction> _actions;

        public ParallelizeActionDecorator(IEnumerable<ITaskAction> actions)
        {
            _actions = actions;
        }

        public async Task RunAction(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var cancelListenersSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var tasks = _actions.Select(x => x.RunAction(cancelListenersSource.Token)).ToList();
                try
                {
                    await (await Task.WhenAny(tasks));
                }
                finally
                {
                    cancelListenersSource.Cancel();
                }
            }
        }
    }
}
