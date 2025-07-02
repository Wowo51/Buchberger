using System.Collections.Generic;
using System.Collections.Immutable;

namespace BuchbergersAlgorithm
{
    public interface IMonomialComparer : IComparer<Monomial>
    {
        // This interface extends IComparer<Monomial> which already provides Compare method.
        // No additional methods needed for basic functionality here, but it signals the intent
        // for custom monomial comparisons.
        ImmutableList<string> OrderedVariables { get; }
    }
}
