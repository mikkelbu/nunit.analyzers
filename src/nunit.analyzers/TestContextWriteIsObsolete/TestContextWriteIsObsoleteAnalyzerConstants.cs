namespace NUnit.Analyzers.TestContextWriteIsObsolete
{
    internal static class TestContextWriteIsObsoleteAnalyzerConstants
    {
        public const string Title = "The Write methods on TestContext are Obsolete";
        public const string Message = "The Write methods are wrappers on TestContext.Out";
        public const string Description = "Direct Write calls should be replaced with Out.Write.";
    }
}
