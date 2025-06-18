namespace GlassLewis.Api.SystemTests.TestCaseOrdering;

/// <summary>
/// Custom attribute for test ordering
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestPriorityAttribute : Attribute
{
    public int Priority { get; }

    public TestPriorityAttribute(int priority)
    {
        Priority = priority;
    }
}
