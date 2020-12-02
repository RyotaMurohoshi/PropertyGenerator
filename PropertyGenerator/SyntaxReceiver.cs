using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PropertyGenerator
{
    internal class SyntaxReceiver : ISyntaxReceiver
    {
        internal List<FieldDeclarationSyntax> CandidateFields { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                && fieldDeclarationSyntax.AttributeLists.Count > 0)
                CandidateFields.Add(fieldDeclarationSyntax);
        }
    }
}