namespace NUnit.Analyzers.Constants
{
    class AsyncUsageAnalyzerConstants
    {
        internal const string Title = "Find Incorrect async tests";
        internal const string Message =
            "Async test method must have non-generic Task return type when no result is expected";
    }
}
