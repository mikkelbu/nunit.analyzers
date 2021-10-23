using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.Constants;
using NUnit.Analyzers.UseCollectionConstraint;
using NUnit.Framework;

namespace NUnit.Analyzers.Tests.UseCollectionConstraint
{
    [TestFixture]
    public sealed class UseCollectionConstraintCodeFixTests
    {
        private static readonly DiagnosticAnalyzer analyzer = new UseCollectionConstraintAnalyzer();
        private static readonly CodeFixProvider fix = new UseCollectionConstraintCodeFix();
        private static readonly ExpectedDiagnostic expectedDiagnostic =
            ExpectedDiagnostic.Create(AnalyzerIdentifiers.UsePropertyConstraint);

        private static readonly string[] NumericContraints =
        {
            "EqualTo(3)",
            "GreaterThan(1)",
            "LessThan(5)",
            "GreaterThanOrEqualTo(3).And.LessThanOrEqualTo(9)",
        };

        [Test]
        public void VerifyGetFixableDiagnosticIds()
        {
            var fix = new UseCollectionConstraintCodeFix();
            var ids = fix.FixableDiagnosticIds;

            Assert.That(ids, Is.EquivalentTo(new[] { AnalyzerIdentifiers.UsePropertyConstraint }));
        }

        [TestCaseSource(nameof(NumericContraints))]
        public void VerifyLength(string constraint)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var array = new int[] {{ 1 }};
            Assert.That(↓array.Length, Is.{constraint});
        }}");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var array = new int[] {{ 1 }};
            Assert.That(array, Has.Length.{constraint});
        }}");
            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode);
        }

        [TestCaseSource(nameof(NumericContraints))]
        public void VerifyCount(string constraint)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var list = new List<int>() {{ 1 }};
            Assert.That(↓list.Count, Is.Not.{constraint}, ""Number of Members"");
        }}", "using System.Collections.Generic;");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var list = new List<int>() {{ 1 }};
            Assert.That(list, Has.Count.Not.{constraint}, ""Number of Members"");
        }}", "using System.Collections.Generic;");
            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode);
        }

        [TestCase("Is.Zero")]
        [TestCase("Is.EqualTo(0)")]
        [TestCase("Is.LessThan(1)")]
        public void VerifyIsEmpty(string constraint)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@$"
        public void TestMethod()
        {{
            Assert.That(↓Array.Empty<int>().Length, {constraint});
        }}");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void TestMethod()
        {
            Assert.That(Array.Empty<int>(), Is.Empty);
        }");
            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode);
        }

        [TestCase("Is.Not.Zero")]
        [TestCase("Is.Positive")]
        [TestCase("Is.Not.EqualTo(0)")]
        [TestCase("Is.Not.LessThan(1)")]
        [TestCase("Is.GreaterThan(0)")]
        [TestCase("Is.GreaterThanOrEqualTo(1)")]
        public void VerifyNotEmpty(string constraint)
        {
            var code = TestUtility.WrapMethodInClassNamespaceAndAddUsings($@"
        public void TestMethod()
        {{
            var list = new List<int>() {{ 1 }};
            Assert.That(↓list.Count, {constraint});
        }}", "using System.Collections.Generic;");
            var fixedCode = TestUtility.WrapMethodInClassNamespaceAndAddUsings(@"
        public void TestMethod()
        {
            var list = new List<int>() { 1 };
            Assert.That(list, Is.Not.Empty);
        }", "using System.Collections.Generic;");
            RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode);
        }
    }
}
