using Microsoft.Practices.Unity;

namespace IDT.TaskActions.Unity
{
    public class UnityHierarchicalScopedAction<TAction> : UnityContainerScopedAction<TAction>
        where TAction : ITaskAction
    {
        public UnityHierarchicalScopedAction(IUnityContainer container)
            : base(container.CreateChildContainer)
        {
        }
    }
}
