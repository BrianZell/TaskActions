using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

namespace IDT.TaskActions.Tests
{
    public class TaskActionFixture : Fixture
    {
        public TaskActionFixture()
        {
            this.Customize(new AutoNSubstituteCustomization());
            this.Register<MockTimeLimitAction>(() => new MockTimeLimitAction(TimeSpan.FromSeconds(1.0)));
        }
    }
}
