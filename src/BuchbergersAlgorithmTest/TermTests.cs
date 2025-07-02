using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System;

namespace BuchbergersAlgorithmTest
{
    [TestClass]
    public sealed class TermTests
    {
        [TestMethod]
        public void Term_Constructor_ValidInput_SetsProperties()
        {
            Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Term term = new Term(5.0, mono);
            Assert.AreEqual(5.0, term.Coefficient, 0.0001);
            Assert.AreEqual(mono, term.Monomial);
        }

        [TestMethod]
        public void Term_Zero_ReturnsZeroCoefficientAndMonomialOne()
        {
            Term zeroTerm = Term.Zero;
            Assert.AreEqual(0.0, zeroTerm.Coefficient, 0.0001);
            Assert.AreEqual(Monomial.One, zeroTerm.Monomial);
        }

        [TestMethod]
        public void Term_Multiply_Scalar_ReturnsNewTermWithMultipliedCoefficient()
        {
            Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } })); // x
            Term term = new Term(2.0, mono); // 2x
            Term result = term.Multiply(3.0); // 6x
            Assert.AreEqual(6.0, result.Coefficient, 0.0001);
            Assert.AreEqual(mono, result.Monomial);
        }

        [TestMethod]
        public void Term_Multiply_Monomial_ReturnsNewTermWithMultipliedMonomial()
        {
            Monomial mono1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } })); // x
            Monomial mono2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } })); // y
            Term term = new Term(2.0, mono1); // 2x
            Term result = term.Multiply(mono2); // 2xy

            Monomial expectedMono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }));
            Assert.AreEqual(2.0, result.Coefficient, 0.0001);
            Assert.AreEqual(expectedMono, result.Monomial);
        }

        [TestMethod]
        public void Term_Multiply_Term_ReturnsNewTermWithProduct()
        {
            Monomial mono1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } })); // x
            Monomial mono2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } })); // y
            Term term1 = new Term(2.0, mono1); // 2x
            Term term2 = new Term(3.0, mono2); // 3y
            Term result = term1.Multiply(term2); // 6xy

            Monomial expectedMono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }));
            Assert.AreEqual(6.0, result.Coefficient, 0.0001);
            Assert.AreEqual(expectedMono, result.Monomial);
        }

        [TestMethod]
        public void Term_Equals_SameObject_ReturnsTrue()
        {
            Monomial mono = Monomial.One;
            Term t1 = new Term(5.0, mono);
            Assert.IsTrue(t1.Equals(t1));
        }

        [TestMethod]
        public void Term_Equals_EqualContent_ReturnsTrue()
        {
            Monomial mono1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial mono2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Term t1 = new Term(5.0, mono1);
            Term t2 = new Term(5.0, mono2);
            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void Term_Equals_DifferentCoefficient_ReturnsFalse()
        {
            Monomial mono = Monomial.One;
            Term t1 = new Term(5.0, mono);
            Term t2 = new Term(6.0, mono);
            Assert.IsFalse(t1.Equals(t2));
        }

        [TestMethod]
        public void Term_Equals_DifferentMonomial_ReturnsFalse()
        {
            Monomial mono1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial mono2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } }));
            Term t1 = new Term(5.0, mono1);
            Term t2 = new Term(5.0, mono2);
            Assert.IsFalse(t1.Equals(t2));
        }

        [TestMethod]
        public void Term_Equals_Null_ReturnsFalse()
        {
            Monomial mono = Monomial.One;
            Term t1 = new Term(5.0, mono);
            Assert.IsFalse(t1.Equals(null));
        }

        [TestMethod]
        public void Term_GetHashCode_EqualTerms_ReturnSameHashCode()
        {
            Monomial mono1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial mono2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Term t1 = new Term(5.0, mono1);
            Term t2 = new Term(5.0, mono2);
            Assert.AreEqual(t1.GetHashCode(), t2.GetHashCode());
        }

        [TestMethod]
        public void Term_ToString_PositiveCoefficientMonomialOne_ReturnsCoefficient()
        {
            Term term = new Term(5.0, Monomial.One);
            Assert.AreEqual("5", term.ToString());
        }

        [TestMethod]
        public void Term_ToString_NegativeCoefficientMonomialOne_ReturnsCoefficient()
        {
            Term term = new Term(-5.0, Monomial.One);
            Assert.AreEqual("-5", term.ToString());
        }

        [TestMethod]
        public void Term_ToString_CoefficientOneMonomial_ReturnsMonomialString()
        {
            Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Term term = new Term(1.0, mono);
            Assert.AreEqual("x^2", term.ToString());
        }

        [TestMethod]
        public void Term_ToString_CoefficientNegativeOneMonomial_ReturnsNegativeMonomialString()
        {
            Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Term term = new Term(-1.0, mono);
            Assert.AreEqual("-x^2", term.ToString());
        }

        [TestMethod]
        public void Term_ToString_ZeroCoefficient_ReturnsZeroString()
        {
            Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Term term = new Term(0.0, mono);
            Assert.AreEqual("0", term.ToString());
        }

        [TestMethod]
        public void Term_ToString_GeneralCase_ReturnsCorrectString()
        {
            Monomial mono = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 3 } })); // xy^3
            Term term = new Term(2.5, mono); // 2.5xy^3
            Assert.AreEqual("2.5xy^3", term.ToString());
        }
    }
}
