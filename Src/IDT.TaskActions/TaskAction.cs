using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class TaskAction : ITaskAction
    {
        private readonly Func<CancellationToken,Task> _action;

        public TaskAction(Action<CancellationToken> action)
            :this((c) =>{
                          action(c);
                          return Task.FromResult(string.Empty);
                      })
        {
        }

        public TaskAction(Func<CancellationToken,Task> action)
        {
            _action = action;
        }

        public async Task RunAction(CancellationToken cancellationToken)
        {
            await _action(cancellationToken);
        }
    }
}
