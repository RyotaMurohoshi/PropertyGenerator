using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace PropertyGenerator
{
    [Generator]
    public class PropertyGenerator : ISourceGenerator
    {
        private string AttributeText => @"
using System;
namespace PropertyGenerator
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class GetterPropertyAttribute : Attribute
    {
        public GetterPropertyAttribute()
        {
        }
        public string PropertyName { get; set; }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("GetterPropertyAttribute", SourceText.From(AttributeText, Encoding.UTF8));

            if (!(context.SyntaxReceiver is SyntaxReceiver receiver)) return;

            var options = (context.Compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation =
                context.Compilation.AddSyntaxTrees(
                    CSharpSyntaxTree.ParseText(SourceText.From(AttributeText, Encoding.UTF8), options));

            var attributeSymbol = compilation.GetTypeByMetadataName("PropertyGenerator.GetterPropertyAttribute");

            var fieldSymbols = new List<IFieldSymbol>();
            foreach (var field in receiver.CandidateFields)
            {
                var model = compilation.GetSemanticModel(field.SyntaxTree);
                foreach (var variable in field.Declaration.Variables)
                {
                    var fieldSymbol = model.GetDeclaredSymbol(variable) as IFieldSymbol;
                    if (fieldSymbol != null && fieldSymbol.GetAttributes().Any(ad =>
                        ad.AttributeClass != null &&
                        ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                        fieldSymbols.Add(fieldSymbol);
                }
            }

            foreach (var group in fieldSymbols.GroupBy(f => f.ContainingType))
            {
                var classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, context);
                context.AddSource($"{group.Key.Name}_GetterPropertyAttribute.cs",
                    SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol,
            GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
                return null;

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            var source = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial class {classSymbol.Name}
    {{
");
            foreach (var fieldSymbol in fields) ProcessField(source, fieldSymbol, attributeSymbol);

            source.Append("} }");

            return source.ToString();
        }

        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            var fieldName = fieldSymbol.Name;
            var fieldType = fieldSymbol.Type;

            var attributeData = fieldSymbol.GetAttributes()
                .Single(ad =>
                    ad.AttributeClass != null &&
                    ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)
                );
            var overridenNameOpt = attributeData
                .NamedArguments
                .SingleOrDefault(kvp => kvp.Key == "PropertyName")
                .Value;

            var propertyName = Utility.ChooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName) return;

            source.Append($"public {fieldType} {propertyName} => this.{fieldName};\n");
        }
    }
}