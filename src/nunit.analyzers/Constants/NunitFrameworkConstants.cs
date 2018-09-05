namespace NUnit.Analyzers.Constants
{
    /// <summary>
    /// String constants for relevant NUnit concepts (classes, fields, properties etc.)
    /// so that we do not need have a dependency on NUnit from the analyzer project.
    /// </summary>
    public static class NunitFrameworkConstants
    {
        public const string NameOfEqualConstraintWithin = "Within";

        public const string NameOfIs = "Is";
        public const string NameOfIsFalse = "False";
        public const string NameOfIsTrue = "True";
        public const string NameOfIsEqualTo = "EqualTo";
        public const string NameOfIsNot = "Not";
        public const string NameOfIsNotEqualTo = "EqualTo";

        public const string NameOfAssert = "Assert";
        public const string NameOfAssertIsTrue = "IsTrue";
        public const string NameOfAssertTrue = "True";
        public const string NameOfAssertIsFalse = "IsFalse";
        public const string NameOfAssertFalse = "False";
        public const string NameOfAssertAreEqual = "AreEqual";
        public const string NameOfAssertAreNotEqual = "AreNotEqual";
        public const string NameOfAssertThat = "That";

        public const string FullNameOfTypeTestCaseAttribute = "NUnit.Framework.TestCaseAttribute";
        public const string FullNameOfTypeTestAttribute = "NUnit.Framework.TestAttribute";

        public const string NameOfTestCaseAttribute = "TestCaseAttribute";
        public const string NameOfTestAttribute = "TestAttribute";
        public const string NameOfTestCaseAttributeExpectedResult = "ExpectedResult";

        public const string AssemblyQualifiedNameOfTypeAssert =
            "NUnit.Framework.Assert, nunit.framework, Version=3.10.1.0, Culture=neutral, PublicKeyToken=2638cd05610744eb";
    }
}
