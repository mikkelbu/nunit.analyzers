using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.DiagnosticSuppressors;
using NUnit.Framework;

namespace NUnit.Analyzers.Tests.DiagnosticSuppressors
{
    public class NonNullableFieldOrPropertyIsUninitializedSuppressorTests
    {
        private static readonly DiagnosticSuppressor suppressor = new NonNullableFieldOrPropertyIsUninitializedSuppressor();

        [Test]
        public async Task FieldNotAssigned()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
                #nullable enable

                private string field;

                [Test]
                public void Test()
                {
                    field = string.Empty;
                    Assert.That(field, Is.Not.Null);
                }
            ");

            await DiagnosticsSuppressorAnalyzer.EnsureNotSuppressed(suppressor, testCode)
                                               .ConfigureAwait(false);
        }

        [TestCase("SetUp", "")]
        [TestCase("OneTimeSetUp", "this.")]
        public async Task FieldAssigned(string attribute, string prefix)
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@$"
                #nullable enable

                private string field;

                [{attribute}]
                public void Initialize()
                {{
                    {prefix}field = string.Empty;
                }}

                [Test]
                public void Test()
                {{
                    Assert.That({prefix}field, Is.Not.Null);
                }}
            ");

            await DiagnosticsSuppressorAnalyzer.EnsureSuppressed(suppressor,
                NonNullableFieldOrPropertyIsUninitializedSuppressor.NullableFieldOrPropertyInitializedInSetUp, testCode)
                .ConfigureAwait(false);
        }

        [TestCase("SetUp", "")]
        [TestCase("OneTimeSetUp", "this.")]
        public async Task FieldAssignedUsingExpressionBody(string attribute, string prefix)
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@$"
                #nullable enable

                private string field;

                [{attribute}]
                public void Initialize() => {prefix}field = string.Empty;

                [Test]
                public void Test() => Assert.That({prefix}field, Is.Not.Null);
            ");

            await DiagnosticsSuppressorAnalyzer.EnsureSuppressed(suppressor,
                NonNullableFieldOrPropertyIsUninitializedSuppressor.NullableFieldOrPropertyInitializedInSetUp, testCode)
                .ConfigureAwait(false);
        }

        [TestCase("SetUp", "")]
        [TestCase("OneTimeSetUp", "this.")]
        public async Task FieldNotAssignedInConstructor(string attribute, string prefix)
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@$"
                #nullable enable

                private string testName;
                private string nunitField;

                public TestClass(string name)
                {{
                    {prefix}testName = name;
                }}

                [{attribute}]
                public void Initialize()
                {{
                    {prefix}nunitField = string.Empty;
                }}

                [Test]
                public void Test()
                {{
                    Assert.That({prefix}nunitField, Is.Not.Null);
                }}
            ");

            await DiagnosticsSuppressorAnalyzer.EnsureSuppressed(suppressor,
                NonNullableFieldOrPropertyIsUninitializedSuppressor.NullableFieldOrPropertyInitializedInSetUp, testCode)
                .ConfigureAwait(false);
        }

        [Test]
        public async Task PropertyNotAssigned()
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@$"
                #nullable enable

                protected string Property {{ get; private set; }}

                [Test]
                public void Test()
                {{
                    Assert.That(Property, Is.Not.Null);
                }}
            ");

            await DiagnosticsSuppressorAnalyzer.EnsureNotSuppressed(suppressor, testCode)
                                               .ConfigureAwait(false);
        }

        [TestCase("SetUp", "")]
        [TestCase("OneTimeSetUp", "this.")]
        public async Task PropertyAssigned(string attribute, string prefix)
        {
            var testCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@$"
                #nullable enable

                protected string Property {{ get; private set; }}

                [{attribute}]
                public void Initialize()
                {{
                    {prefix}Property = string.Empty;
                }}

                [Test]
                public void Test()
                {{
                    Assert.That({prefix}Property, Is.Not.Null);
                }}
            ");

            await DiagnosticsSuppressorAnalyzer.EnsureSuppressed(suppressor,
                NonNullableFieldOrPropertyIsUninitializedSuppressor.NullableFieldOrPropertyInitializedInSetUp, testCode)
                .ConfigureAwait(false);
        }
    }
}
