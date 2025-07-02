using System.Collections.Generic;
using System.Collections.Immutable;
using System; // For Object.ReferenceEquals

namespace BuchbergersAlgorithm
{
    // Lexicographic Order (lex): Monomials are ordered as they would be in a dictionary.
    // For example, x^2 > xy > x > y^2. Requires a fixed order of variables, e.g., x > y > z.
    public sealed class LexicographicComparer : IMonomialComparer
    {
        private readonly ImmutableList<string> _orderedVariables;

        public LexicographicComparer(ImmutableList<string> orderedVariables)
        {
            _orderedVariables = orderedVariables;
        }

        public ImmutableList<string> OrderedVariables
        {
            get { return _orderedVariables; }
        }

        // Compares two monomials for lexicographic order.
        // Returns:
        //   -1: x is less than y
        //    0: x is equal to y
        //    1: x is greater than y
        public int Compare(Monomial? x, Monomial? y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x is null)
            {
                return -1;
            }
            if (y is null)
            {
                return 1;
            }

            foreach (string var in _orderedVariables)
            {
                int xExp = x.GetExponent(var);
                int yExp = y.GetExponent(var);

                if (xExp != yExp)
                {
                    return xExp.CompareTo(yExp); // For lex, larger exponent means larger monomial for that variable
                }
            }
            return 0; // Monomials are equal with respect to the given variables
        }
    }
}
