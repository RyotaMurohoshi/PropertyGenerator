using Microsoft.CodeAnalysis;

namespace PropertyGenerator
{
    internal static class Utility
    {
        internal static string ChooseName(string fieldName, TypedConstant overridenNameOpt)
        {
            if (!overridenNameOpt.IsNull) return overridenNameOpt.Value.ToString();

            fieldName = fieldName.TrimStart('_');

            if (fieldName.Length == 0)
                return string.Empty;

            if (fieldName.Length == 1)
                return fieldName.ToUpper();

            return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
        }
    }
}