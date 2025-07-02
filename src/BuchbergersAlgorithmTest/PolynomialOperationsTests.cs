using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BuchbergersAlgorithmTest
{
    [TestClass]
    public sealed class PolynomialOperationsTests
    {
        private IMonomialComparer _lexComparer = new LexicographicComparer(ImmutableList.Create("x", "y", "z"));

        [TestMethod]
        public void CalculateSPolynomial_SimpleCase_EliminatesLeadingTerms()
        {
            // f = x^2y - 1
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 }, { "y", 1 } }), (-1.0, new Dictionary<string, int> { }));
            // g = xy^2 - x
            Polynomial g = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }), (-1.0, new Dictionary<string, int> { { "x", 1 } }));

            // S(f,g) = x^2 - y
            Polynomial expectedSPolynomial = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 } }), (-1.0, new Dictionary<string, int> { { "y", 1 } }));

            Polynomial sPoly = PolynomialOperations.CalculateSPolynomial(f, g, _lexComparer);

            Assert.IsTrue(expectedSPolynomial.Equals(sPoly), $"Expected S-polynomial: {expectedSPolynomial}, Actual: {sPoly}");
        }

        [TestMethod]
        public void CalculateSPolynomial_WithZeroPolynomial_ReturnsZeroPolynomial()
        {
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }));
            Polynomial g = new Polynomial(); // Zero polynomial

            Polynomial sPoly = PolynomialOperations.CalculateSPolynomial(f, g, _lexComparer);
            Assert.IsTrue(sPoly.IsZero);

            sPoly = PolynomialOperations.CalculateSPolynomial(g, f, _lexComparer);
            Assert.IsTrue(sPoly.IsZero);
        }

        [TestMethod]
        public void Reduce_SimpleReduction_ReducesToZero()
        {
            // f = x^2 - y, g1 = x^2 - y
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 } }), (-1.0, new Dictionary<string, int> { { "y", 1 } }));
            Polynomial g1 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 } }), (-1.0, new Dictionary<string, int> { { "y", 1 } }));

            ImmutableList<Polynomial> G = ImmutableList.Create(g1);

            Polynomial remainder = PolynomialOperations.Reduce(f, G, _lexComparer);
            Assert.IsTrue(remainder.IsZero);
        }

        [TestMethod]
        public void Reduce_MultipleStepReduction()
        {
            // f = x^2y + y
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 2 }, { "y", 1 } }), (1.0, new Dictionary<string, int> { { "y", 1 } }));
            // g1 = xy - 1
            Polynomial g1 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }), (-1.0, new Dictionary<string, int> { }));
            // g2 = x + y^2
            Polynomial g2 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }), (1.0, new Dictionary<string, int> { { "y", 2 } }));

            ImmutableList<Polynomial> G = ImmutableList.Create(g1, g2);

            // Expected remainder: y - y^2
            Polynomial expectedRemainder = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "y", 1 } }), (-1.0, new Dictionary<string, int> { { "y", 2 } }));

            Polynomial remainder = PolynomialOperations.Reduce(f, G, _lexComparer);
            Assert.IsTrue(expectedRemainder.Equals(remainder), $"Expected remainder: {expectedRemainder}, Actual: {remainder}");
        }

        [TestMethod]
        public void Reduce_NoReductionPossible_ReturnsOriginal()
        {
            // f = y^2
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "y", 2 } }));
            // g = x - 1
            Polynomial g = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }), (-1.0, new Dictionary<string, int> { }));

            ImmutableList<Polynomial> G = ImmutableList.Create(g);

            Polynomial remainder = PolynomialOperations.Reduce(f, G, _lexComparer);
            Assert.IsTrue(f.Equals(remainder));
        }

        [TestMethod]
        public void Reduce_BasisContainsZeroPolynomial_HandlesGracefully()
        {
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }));
            Polynomial g1 = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }));
            Polynomial g2 = new Polynomial(); // Zero polynomial

            ImmutableList<Polynomial> G = ImmutableList.Create(g1, g2);

            Polynomial remainder = PolynomialOperations.Reduce(f, G, _lexComparer);
            Assert.IsTrue(remainder.IsZero); // Should still reduce to zero
        }

        [TestMethod]
        public void Reduce_EmptyBasis_ReturnsOriginalPolynomial()
        {
            Polynomial f = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }));
            ImmutableList<Polynomial> emptyG = ImmutableList<Polynomial>.Empty;

            Polynomial remainder = PolynomialOperations.Reduce(f, emptyG, _lexComparer);
            Assert.IsTrue(f.Equals(remainder));
        }

        [TestMethod]
        public void Reduce_ZeroPolynomialAsInput_ReturnsZeroPolynomial()
        {
            Polynomial zeroF = new Polynomial();
            Polynomial g = TestPolynomialGenerator.CreatePolynomial((1.0, new Dictionary<string, int> { { "x", 1 } }));
            ImmutableList<Polynomial> G = ImmutableList.Create(g);

            Polynomial remainder = PolynomialOperations.Reduce(zeroF, G, _lexComparer);
            Assert.IsTrue(remainder.IsZero);
        }
    }
}
