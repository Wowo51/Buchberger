using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BuchbergersAlgorithmTest
{
    public static class TestPolynomialGenerator
    {
        private static readonly Random _random = new Random();

        public static Monomial GenerateRandomMonomial(ImmutableList<string> variables, int maxDegree)
        {
            Dictionary<string, int> exponents = new Dictionary<string, int>();
            int currentTotalDegree = 0;

            foreach (string var in variables)
            {
                if (currentTotalDegree >= maxDegree)
                {
                    break;
                }
                int maxExpForVar = maxDegree - currentTotalDegree;
                int exponent = _random.Next(0, maxExpForVar + 1); // 0 to maxExpForVar inclusive
                if (exponent > 0)
                {
                    exponents.Add(var, exponent);
                    currentTotalDegree += exponent;
                }
            }
            return new Monomial(ImmutableSortedDictionary.CreateRange(exponents));
        }

        public static Term GenerateRandomTerm(ImmutableList<string> variables, int maxDegree, double maxCoefficient)
        {
            double coefficient = _random.NextDouble() * (2 * maxCoefficient) - maxCoefficient; // Between -maxCoeff and +maxCoeff
            Monomial monomial = GenerateRandomMonomial(variables, maxDegree);
            return new Term(coefficient, monomial);
        }

        public static Polynomial GenerateRandomPolynomial(ImmutableList<string> variables, int maxTerms, int maxDegree, double maxCoefficient)
        {
            int numberOfTerms = _random.Next(1, maxTerms + 1); // At least one term
            List<Term> terms = new List<Term>();
            for (int i = 0; i < numberOfTerms; i++)
            {
                terms.Add(GenerateRandomTerm(variables, maxDegree, maxCoefficient));
            }
            return new Polynomial(terms);
        }

        // Helper to create a polynomial from terms
        public static Polynomial CreatePolynomial(params (double coeff, IReadOnlyDictionary<string, int> monoExponents)[] termsData)
        {
            List<Term> terms = new List<Term>();
            foreach (var td in termsData)
            {
                Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(td.monoExponents));
                terms.Add(new Term(td.coeff, mono));
            }
            return new Polynomial(terms);
        }

        // Helper to verify if a given basis G is a Gröbner basis
        public static bool VerifyIsGroebnerBasis(ImmutableList<Polynomial> G, IMonomialComparer comparer)
        {
            G = ImmutableList.CreateRange(G.Where(p => p.IsZero == false)); // Filter out zero polynomials
            if (G.Count <= 1)
            {
                return true; // Basis with 0 or 1 non-zero polynomial is always a Gröbner basis.
            }

            for (int i = 0; i < G.Count; i++)
            {
                for (int j = i + 1; j < G.Count; j++)
                {
                    Polynomial sPolynomial = PolynomialOperations.CalculateSPolynomial(G[i], G[j], comparer);
                    Polynomial reducedSPolynomial = PolynomialOperations.Reduce(sPolynomial, G, comparer);
                    if (reducedSPolynomial.IsZero == false)
                    {
                        Console.WriteLine($"\nVerification failed for pair ({G[i]}, {G[j]}):");
                        Console.WriteLine($"S-Poly: {sPolynomial}");
                        Console.WriteLine($"Reduced S-Poly (should be zero): {reducedSPolynomial}");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
