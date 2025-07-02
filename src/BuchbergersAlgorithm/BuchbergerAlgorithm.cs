using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BuchbergersAlgorithm
{
    public static class BuchbergerAlgorithm
    {
        public static ImmutableList<Polynomial> ComputeGroebnerBasis(List<Polynomial> initialBasis, IMonomialComparer comparer)
        {
            if (initialBasis is null || initialBasis.Count == 0)
            {
                return ImmutableList<Polynomial>.Empty;
            }

            // Deep copy the initial basis to avoid modifying the input list
            // Also filter out any initial zero polynomials or normalize them.
            List<Polynomial> G = initialBasis.Where(p => p.IsZero == false).Select(p => p).ToList();

            // Set of pairs {p, q} of polynomials from G to check
            Queue<Tuple<Polynomial, Polynomial>> P = new Queue<Tuple<Polynomial, Polynomial>>();

            // Initialize P with all unique pairs from G
            for (int i = 0; i < G.Count; i++)
            {
                for (int j = i + 1; j < G.Count; j++)
                {
                    P.Enqueue(Tuple.Create(G[i], G[j]));
                }
            }

            while (P.Count > 0)
            {
                Tuple<Polynomial, Polynomial> pair = P.Dequeue();
                Polynomial p = pair.Item1;
                Polynomial q = pair.Item2;

                Polynomial sPolynomial = PolynomialOperations.CalculateSPolynomial(p, q, comparer);

                // Reduce S-polynomial by the current basis G
                Polynomial h = PolynomialOperations.Reduce(sPolynomial, ImmutableList.CreateRange(G), comparer);

                if (h.IsZero == false)
                {
                    // Add the non-zero remainder h to the basis G
                    G.Add(h);

                    // Form new pairs by pairing the new element h with all existing polynomials in G
                    // (excluding h itself, which is already the last element added).
                    int newGIndex = G.Count - 1; // Index of the newly added polynomial h
                    for (int i = 0; i < newGIndex; i++) // Pair h with all elements in G that existed before h
                    {
                        P.Enqueue(Tuple.Create(G[i], h));
                    }
                }
            }

            // The final set G is a Gröbner basis requiring post-processing for reduction and normalization.
            return ReduceAndNormalizeGroebnerBasis(G, comparer);
        }

        /// <summary>
        /// Reduces and normalizes a raw Gröbner basis to obtain a reduced Gröbner basis.
        /// This involves making all non-zero polynomials monic and iteratively removing redundant polynomials.
        /// </summary>
        /// <param name="rawGroebnerBasis">The raw Gröbner basis computed by Buchberger's algorithm.</param>
        /// <param name="comparer">The monomial comparer to use for ordering and reduction.</param>
        /// <returns>An immutable list representing the reduced Gröbner basis.</returns>
        private static ImmutableList<Polynomial> ReduceAndNormalizeGroebnerBasis(List<Polynomial> rawGroebnerBasis, IMonomialComparer comparer)
        {
            List<Polynomial> currentBasis = rawGroebnerBasis.ToList();

            // Step 1: Make all non-zero polynomials monic and filter out any zeros
            // This ensures each polynomial's leading coefficient is 1 and removes any zero polynomials.
            currentBasis = currentBasis
                .Where((Polynomial p) => !p.IsZero)
                .Select((Polynomial p) => PolynomialOperations.MakeMonic(p, comparer))
                .ToList();

            // Sort the initial basis once based on their leading monomials (using the provided comparer)
            // This provides a consistent starting point for the iterative reduction process.
            currentBasis.Sort((Polynomial p1, Polynomial p2) =>
            {
                // Handle nulls and reference equality for consistent sorting.
                if (System.Object.ReferenceEquals(p1, p2)) { return 0; }
                if (p1 is null) { return 1; } // Nulls come after non-nulls
                if (p2 is null) { return -1; } // Non-nulls come before nulls
                // Handle zero polynomials for consistent sorting, although they should be filtered out already by the Where clause above.
                if (p1.IsZero && p2.IsZero) { return 0; }
                if (p1.IsZero) { return 1; } // Zero polynomials come last
                if (p2.IsZero) { return -1; } // Non-zero polynomials come first
                
                return comparer.Compare(p1.GetLeadingMonomial(comparer), p2.GetLeadingMonomial(comparer));
            });

            // Step 2: Iteratively remove redundant polynomials and fully reduce others
            // A polynomial p in the basis is redundant if it can be reduced to the zero polynomial
            // by the other polynomials in the basis. This loop continues until no more changes occur.
            bool changed;
            do
            {
                changed = false;
                // Take a snapshot of the current state of the basis, which is already sorted from the previous iteration or initial sort.
                List<Polynomial> previousBasis = new List<Polynomial>(currentBasis); 
                List<Polynomial> nextBasisCandidates = new List<Polynomial>();

                foreach (Polynomial p in previousBasis)
                {
                    // Create basis for reduction excluding the current polynomial 'p'.
                    // The relative order from previousBasis is implicitly maintained by Where and ToImmutableList.
                    ImmutableList<Polynomial> basisForReduction = previousBasis
                        .Where((Polynomial other) => !System.Object.ReferenceEquals(p, other)) // Exclude p itself
                        .ToImmutableList();

                    // Reduce 'p' using the rest of the basis.
                    Polynomial reducedP = PolynomialOperations.Reduce(p, basisForReduction, comparer);
                    reducedP = PolynomialOperations.MakeMonic(reducedP, comparer); // Ensure the reduced form is monic.

                    if (!reducedP.IsZero)
                    {
                        // If p reduces to a non-zero polynomial, add its reduced, monic form to the new basis candidates.
                        // If reducedP is zero, it means p was redundant, so we do not add it.
                        nextBasisCandidates.Add(reducedP);
                    }
                }

                // Get distinct polynomials from the candidates and sort them for consistent comparison.
                List<Polynomial> distinctAndSortedNextBasis = nextBasisCandidates.Distinct().ToList();
                distinctAndSortedNextBasis.Sort((Polynomial p1, Polynomial p2) =>
                {
                    // Handle nulls and reference equality for consistent sorting.
                    if (System.Object.ReferenceEquals(p1, p2)) { return 0; }
                    if (p1 is null) { return 1; } // Nulls come after non-nulls
                    if (p2 is null) { return -1; } // Non-nulls come before nulls
                    // Handle zero polynomials for consistent sorting.
                    if (p1.IsZero && p2.IsZero) { return 0; }
                    if (p1.IsZero) { return 1; }
                    if (p2.IsZero) { return -1; }

                    return comparer.Compare(p1.GetLeadingMonomial(comparer), p2.GetLeadingMonomial(comparer));
                });

                // Compare the previous sorted basis with the newly processed and sorted basis.
                // SequenceEqual performs element-wise comparison on sorted lists, leveraging Polynomial.Equals().
                if (!previousBasis.SequenceEqual(distinctAndSortedNextBasis))
                {
                    changed = true;
                }

                currentBasis = distinctAndSortedNextBasis; // Update 'currentBasis' for the next iteration (if changed) or final return.

            } while (changed); // Continue until no more changes (no polynomials removed, added or significantly altered)

            return ImmutableList.CreateRange(currentBasis);
        }
    }
}