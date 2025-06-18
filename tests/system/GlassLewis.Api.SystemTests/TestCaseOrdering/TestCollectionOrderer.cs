using Xunit.Abstractions;
using Xunit.Sdk;

namespace GlassLewis.Api.SystemTests.TestCaseOrdering;

public class TestCaseOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(testCase => GetTestPriority(testCase));
    }

    private static int GetTestPriority(ITestCase testCase)
    {
        var priorityAttribute = testCase.TestMethod.Method
            .GetCustomAttributes(typeof(TestPriorityAttribute))
            .FirstOrDefault();

        if (priorityAttribute != null)
        {
            var constructorArgs = priorityAttribute.GetConstructorArguments().ToArray();
            if (constructorArgs.Length > 0 && constructorArgs[0] is int priority)
            {
                return priority;
            }
        }

        return int.MaxValue; // No priority = run last
    }
}
