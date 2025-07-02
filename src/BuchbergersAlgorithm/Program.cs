using BuchbergersAlgorithm;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BuchbergersAlgorithm
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Buchberger's Algorithm Example");

            // Define ordered variables for lexicographic order (x > y) as specified in the example
            ImmutableList<string> orderedVariables = ImmutableList.Create("x", "y");
            IMonomialComparer lexComparer = new LexicographicComparer(orderedVariables);

            // Define polynomials f1 = x^2y - 1 and f2 = xy^2 - x
            // f1 = 1x^2y^1 - 1x^0y^0
            ImmutableSortedDictionary<string, int> exponents_f1_t1 = ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } });
            Monomial mono_f1_t1 = new Monomial(exponents_f1_t1);
            Term term_f1_t1 = new Term(1.0, mono_f1_t1);
            Term term_f1_t2 = new Term(-1.0, Monomial.One); // Constant -1
            Polynomial f1 = new Polynomial(new Term[] { term_f1_t1, term_f1_t2 });

            // f2 = 1x^1y^2 - 1x^1y^0
            ImmutableSortedDictionary<string, int> exponents_f2_t1 = ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } });
            Monomial mono_f2_t1 = new Monomial(exponents_f2_t1);
            Term term_f2_t1 = new Term(1.0, mono_f2_t1);
            ImmutableSortedDictionary<string, int> exponents_f2_t2 = ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } });
            Monomial mono_f2_t2 = new Monomial(exponents_f2_t2);
            Term term_f2_t2 = new Term(-1.0, mono_f2_t2);
            Polynomial f2 = new Polynomial(new Term[] { term_f2_t1, term_f2_t2 });

            List<Polynomial> initialBasis = new List<Polynomial> { f1, f2 };

            Console.WriteLine($"Initial Basis F: [ {f1}, {f2} ]");

            ImmutableList<Polynomial> groebnerBasis = BuchbergerAlgorithm.ComputeGroebnerBasis(initialBasis, lexComparer);

            Console.WriteLine("\nComputed Gröbner Basis G:");
            foreach (Polynomial p in groebnerBasis)
            {
                Console.WriteLine($"- {p}");
            }
        }
    }
}
