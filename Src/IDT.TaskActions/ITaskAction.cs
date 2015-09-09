using System.Threading;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public interface ITaskAction
    {
        Task RunAction(CancellationToken cancellationToken);
    }
}
