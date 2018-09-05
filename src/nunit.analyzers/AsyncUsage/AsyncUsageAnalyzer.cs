using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.Constants;

namespace NUnit.Analyzers.AsyncUsage
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AsyncUsageAnalyzer : DiagnosticAnalyzer
    {
        private const string TaskTypeName = "System.Threading.Tasks.Task";
        private const string AsyncAttributeTypeName = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";

        private static DiagnosticDescriptor CreateDescriptor(string message) =>
            new DiagnosticDescriptor(AnalyzerIdentifiers.AsyncUsage, AsyncUsageAnalyzerConstants.Title,
                message, Categories.Usage,
                DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    AsyncUsageAnalyzer.CreateDescriptor(AsyncUsageAnalyzerConstants.Message));
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AsyncUsageAnalyzer.MethodDeclaration, SyntaxKind.Attribute);
        }

        private static void MethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodNode = context.Node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            if (methodNode != null)
            {
                if (!methodNode.ContainsDiagnostics)
                {
                    var testCaseType = context.SemanticModel.Compilation.GetTypeByMetadataName(NunitFrameworkConstants.FullNameOfTypeTestCaseAttribute);
                    var testType = context.SemanticModel.Compilation.GetTypeByMetadataName(NunitFrameworkConstants.FullNameOfTypeTestAttribute);
                    if (testCaseType == null || testType == null)
                        return;

                    var attributeNode = (AttributeSyntax)context.Node;
                    var attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeNode).Symbol;

                    if ((testCaseType.ContainingAssembly.Identity == attributeSymbol?.ContainingAssembly.Identity &&
                        NunitFrameworkConstants.NameOfTestCaseAttribute == attributeSymbol?.ContainingType.Name) ||
                        (testType.ContainingAssembly.Identity == attributeSymbol?.ContainingAssembly.Identity &&
                        NunitFrameworkConstants.NameOfTestAttribute == attributeSymbol?.ContainingType.Name))
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();

                        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);

                        //methodSymbol.ReturnType
                        
                        //if (attributePositionalArguments.Length < methodRequiredParameters)
                        //{
                        //    context.ReportDiagnostic(Diagnostic.Create(
                        //        TestCaseUsageAnalyzer.CreateDescriptor(
                        //            TestCaseUsageAnalyzerConstants.NotEnoughArgumentsMessage),
                        //        attributeNode.GetLocation()));
                        //}
                    }
                }
            }
        }

        // COPIED FROM C:\src\NUnit\nunit\src\NUnitFramework\framework\Internal\AsyncInvocationRegion.cs
        private static bool IsAsyncOperation(MethodInfo method)
        {
            var name = method.ReturnType.FullName;
            if (name == null) return false;
            return name.StartsWith(TaskTypeName) ||
                   method.GetCustomAttributes(false).Any(attr => AsyncAttributeTypeName == attr.GetType().FullName);
        }
    }
}
