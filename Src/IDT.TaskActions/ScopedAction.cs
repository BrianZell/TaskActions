using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class ScopedAction<TAction> : ITaskAction
        where TAction : ITaskAction
    {
        private readonly ExportFactory<TAction> _scopedFactory;

        public ScopedAction(ExportFactory<TAction> scopedFactory)
        {
            _scopedFactory = scopedFactory;
        }

        public async Task RunAction(CancellationToken cancellationToken)
        {
            using (var scopedItem = _scopedFactory.CreateExport())
            {
                await scopedItem.Value.RunAction(cancellationToken);
            }
        }
    }
}
