using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BuchbergersAlgorithmTest
{
    [TestClass]
    public sealed class BuchbergerAlgorithmTests
    {
        private IMonomialComparer _lexComparer = new LexicographicComparer(ImmutableList.Create("x", "y", "z"));

        [TestMethod]
        public void ComputeGroebnerBasis_SingleVariablePoly_ReturnsLeadingTermPoly()
        {
            // f1 = x^2 - 1, f2 = x - 1
            Polynomial f1 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 } }), (-1.0, new Dictionary<string, int> { }));
            Polynomial f2 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }), (-1.0, new Dictionary<string, int> { }));
            
            List<Polynomial> initialBasis = new List<Polynomial> { f1, f2 };
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x"));

            ImmutableList<Polynomial> groebnerBasis = BuchbergerAlgorithm.ComputeGroebnerBasis(initialBasis, comparer);

            // Expect {x - 1} as the reduced Gröbner basis
            Polynomial expectedBasisElement = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }), (-1.0, new Dictionary<string, int> { }));
            
            Assert.AreEqual(1, groebnerBasis.Count, "The Gröbner basis count should be 1.");
            Assert.IsTrue(groebnerBasis.Any(p => p.Equals(expectedBasisElement) || p.Equals(expectedBasisElement.Multiply(-1.0))), "The Gröbner basis should contain x-1 or 1-x.");
            Assert.IsTrue(TestPolynomialGenerator.VerifyIsGroebnerBasis(groebnerBasis, comparer), "The computed basis must be a Gröbner basis.");
        }

        [TestMethod]
        [Timeout(5 * 60 * 1000)] // 5 minutes timeout
        public void ComputeGroebnerBasis_ClassicExample_CoxLittleOShea_Page90()
        {
            // From "Ideals, Varieties, and Algorithms" by Cox, Little, O'Shea, Chapter 2, Section 8, Example 4.
            // f1 = x^2y - 1, f2 = xy^2 - x
            // Variables: x > y (lexicographic order)

            Polynomial f1 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 }, { "y", 1 } }), (-1.0, new Dictionary<string, int> { }));
            Polynomial f2 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }), (-1.0, new Dictionary<string, int> { { "x", 1 } }));

            List<Polynomial> initialBasis = new List<Polynomial> { f1, f2 };

            ImmutableList<Polynomial> groebnerBasis = BuchbergerAlgorithm.ComputeGroebnerBasis(initialBasis, _lexComparer);
            
            // The algorithm should compute a Gröbner basis. The exact elements might vary depending on simplification rules,
            // but all S-polynomials formed from the computed basis should reduce to zero.
            Assert.IsTrue(TestPolynomialGenerator.VerifyIsGroebnerBasis(groebnerBasis, _lexComparer), "The computed basis must be a Gröbner basis for Cox-Little-O'Shea example.");
            
            Console.WriteLine("\nCox, Little, O'Shea Example Gröbner Basis:");
            if (groebnerBasis.Count > 0)
            {
                foreach (Polynomial p in groebnerBasis)
                {
                    Console.WriteLine($"- {p}");
                }
            }
            else
            {
                Console.WriteLine("Basis is empty (unexpected for this example).");
            }
            Assert.IsTrue(groebnerBasis.Count > 0, "Gröbner basis should not be empty for this example.");
        }

        [TestMethod]
        [Timeout(5 * 60 * 1000)] // 5 minutes timeout
        public void ComputeGroebnerBasis_RandomPolynomials_VerificationTest()
        {
            ImmutableList<string> variables = ImmutableList.Create("x", "y", "z");
            IMonomialComparer comparer = new LexicographicComparer(variables);

            int numberOfInitialPolynomials = 3; // Keep it small to avoid too long computation
            int maxTermsPerPolynomial = 3;
            int maxDegree = 4;
            double maxCoefficient = 10.0;

            List<Polynomial> initialBasis = new List<Polynomial>();
            for (int i = 0; i < numberOfInitialPolynomials; i++)
            {
                initialBasis.Add(TestPolynomialGenerator.GenerateRandomPolynomial(variables, maxTermsPerPolynomial, maxDegree, maxCoefficient));
            }
            
            Console.WriteLine("\nRandom Polynomials Initial Basis:");
            foreach (Polynomial p in initialBasis)
            {
                Console.WriteLine($"- {p}");
            }

            ImmutableList<Polynomial> groebnerBasis = BuchbergerAlgorithm.ComputeGroebnerBasis(initialBasis, comparer);

            Console.WriteLine("\nComputed Gröbner Basis for Random Polynomials:");
            foreach (Polynomial p in groebnerBasis)
            {
                Console.WriteLine($"- {p}");
            }

            Assert.IsTrue(TestPolynomialGenerator.VerifyIsGroebnerBasis(groebnerBasis, comparer), "The computed basis from random polynomials must be a Gröbner basis.");
        }

        [TestMethod]
        public void ComputeGroebnerBasis_EmptyInitialBasis_ReturnsEmptyBasis()
        {
            List<Polynomial> initialBasis = new List<Polynomial>();
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x"));
            ImmutableList<Polynomial> groebnerBasis = BuchbergerAlgorithm.ComputeGroebnerBasis(initialBasis, comparer);
            Assert.AreEqual(0, groebnerBasis.Count);
        }

        [TestMethod]
        public void ComputeGroebnerBasis_BasisContainingZeroPolynomials_HandlesGracefully()
        {
            Polynomial f1 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } })); // x
            Polynomial f2 = new Polynomial(); // 0
            Polynomial f3 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 } })); // x^2

            List<Polynomial> initialBasis = new List<Polynomial> { f1, f2, f3 };
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x"));

            ImmutableList<Polynomial> groebnerBasis = BuchbergerAlgorithm.ComputeGroebnerBasis(initialBasis, comparer);

            // Expected Gröbner basis will be like {x}. Should not include the zero polynomial.
            Polynomial expectedBasisElement = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }));

            Assert.IsTrue(groebnerBasis.Any(p => p.Equals(expectedBasisElement) || p.Equals(expectedBasisElement.Multiply(-1.0))));
            Assert.IsFalse(groebnerBasis.Any(p => p.IsZero));
            Assert.IsTrue(TestPolynomialGenerator.VerifyIsGroebnerBasis(groebnerBasis, comparer), "The computed basis must be a Gröbner basis and correctly handle initial zero polynomials.");
        }
    }
}
