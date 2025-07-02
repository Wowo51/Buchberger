using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BuchbergersAlgorithmTest
{
    [TestClass]
    public sealed class PolynomialTests
    {
        private IMonomialComparer _lexComparer = new LexicographicComparer(ImmutableList.Create("x", "y", "z"));

        [TestMethod]
        public void Polynomial_Constructor_NormalizesTerms_CombinesLikeTerms()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial y = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } }));

            Term xyTerm1 = new Term(2.0, new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } })));
            Term xyTerm2 = new Term(3.0, new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } })));
            Term xTerm = new Term(1.0, x);
            Term yTerm = new Term(4.0, y);

            List<Term> terms = new List<Term> { xyTerm1, xTerm, xyTerm2, yTerm };
            Polynomial poly = new Polynomial(terms);

            Assert.AreEqual(3, poly.Terms.Count); // 5xy, 1x, 4y
            Assert.IsTrue(poly.Terms.Any(t => Math.Abs(t.Coefficient - 5.0) < 0.0001 && t.Monomial.Exponents.ContainsKey("x") && t.Monomial.Exponents.ContainsKey("y")));
            Assert.IsTrue(poly.Terms.Any(t => Math.Abs(t.Coefficient - 1.0) < 0.0001 && t.Monomial.Exponents.ContainsKey("x") && t.Monomial.Exponents.Count == 1));
            Assert.IsTrue(poly.Terms.Any(t => Math.Abs(t.Coefficient - 4.0) < 0.0001 && t.Monomial.Exponents.ContainsKey("y") && t.Monomial.Exponents.Count == 1));
        }

        [TestMethod]
        public void Polynomial_Constructor_NormalizesTerms_RemovesZeroCoefficientTerms()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Term term1 = new Term(2.0, x);
            Term term2 = new Term(-2.0, x);
            Term term3 = new Term(1.0, Monomial.One); // constant

            Polynomial poly = new Polynomial(new Term[] { term1, term2, term3 });
            Assert.AreEqual(1, poly.Terms.Count);
            Assert.IsTrue(Math.Abs(poly.Terms[0].Coefficient - 1.0) < 0.0001);
            Assert.AreEqual(Monomial.One, poly.Terms[0].Monomial);
        }

        [TestMethod]
        public void Polynomial_IsZero_ReturnsTrueForZeroPolynomial()
        {
            Polynomial zeroPoly = new Polynomial();
            Assert.IsTrue(zeroPoly.IsZero);
        }

        [TestMethod]
        public void Polynomial_IsZero_ReturnsFalseForNonZeroPolynomial()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial nonZeroPoly = new Polynomial(new Term[] { new Term(1.0, x) });
            Assert.IsFalse(nonZeroPoly.IsZero);
        }

        [TestMethod]
        public void Polynomial_CreateConstant_ZeroConstant_ReturnsZeroPolynomial()
        {
            Polynomial poly = Polynomial.CreateConstant(0.0);
            Assert.IsTrue(poly.IsZero);
        }

        [TestMethod]
        public void Polynomial_CreateConstant_NonZeroConstant_ReturnsCorrectPolynomial()
        {
            Polynomial poly = Polynomial.CreateConstant(5.0);
            Assert.IsFalse(poly.IsZero);
            Assert.AreEqual(1, poly.Terms.Count);
            Assert.AreEqual(5.0, poly.Terms[0].Coefficient, 0.0001);
            Assert.AreEqual(Monomial.One, poly.Terms[0].Monomial);
        }

        [TestMethod]
        public void Polynomial_GetLeadingTerm_ReturnsCorrectTerm()
        {
            Monomial x2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Monomial xy = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }));
            Monomial y2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 2 } }));

            Term term1 = new Term(3.0, y2); // 3y^2
            Term term2 = new Term(2.0, x2); // 2x^2 (leading for lex x>y)
            Term term3 = new Term(4.0, xy); // 4xy

            Polynomial poly = new Polynomial(new Term[] { term1, term2, term3 });
            Term lt = poly.GetLeadingTerm(_lexComparer);
            Assert.AreEqual(2.0, lt.Coefficient, 0.0001);
            Assert.AreEqual(x2, lt.Monomial);
        }

        [TestMethod]
        public void Polynomial_GetLeadingTerm_ZeroPolynomial_ReturnsZeroTerm()
        {
            Polynomial zeroPoly = new Polynomial();
            Term lt = zeroPoly.GetLeadingTerm(_lexComparer);
            Assert.AreEqual(Term.Zero, lt);
        }

        [TestMethod]
        public void Polynomial_GetLeadingMonomial_ReturnsCorrectMonomial()
        {
            Monomial x2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Monomial xy = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }));
            Term term1 = new Term(3.0, xy);
            Term term2 = new Term(2.0, x2);

            Polynomial poly = new Polynomial(new Term[] { term1, term2 });
            Monomial lm = poly.GetLeadingMonomial(_lexComparer);
            Assert.AreEqual(x2, lm);
        }

        [TestMethod]
        public void Polynomial_GetLeadingMonomial_ZeroPolynomial_ReturnsMonomialOne()
        {
            Polynomial zeroPoly = new Polynomial();
            Monomial lm = zeroPoly.GetLeadingMonomial(_lexComparer);
            Assert.AreEqual(Monomial.One, lm); // Conventionally so it doesn't break division if something divides 0
        }

        [TestMethod]
        public void Polynomial_GetLeadingCoefficient_ReturnsCorrectCoefficient()
        {
            Monomial x2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Monomial xy = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }));
            Term term1 = new Term(3.0, xy);
            Term term2 = new Term(2.0, x2);

            Polynomial poly = new Polynomial(new Term[] { term1, term2 });
            double lc = poly.GetLeadingCoefficient(_lexComparer);
            Assert.AreEqual(2.0, lc, 0.0001);
        }

        [TestMethod]
        public void Polynomial_GetLeadingCoefficient_ZeroPolynomial_ReturnsZero()
        {
            Polynomial zeroPoly = new Polynomial();
            double lc = zeroPoly.GetLeadingCoefficient(_lexComparer);
            Assert.AreEqual(0.0, lc, 0.0001);
        }

        [TestMethod]
        public void Polynomial_Add_NormalCase_ReturnsSum()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial y = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } }));

            Polynomial p1 = new Polynomial(new Term[] { new Term(1.0, x), new Term(2.0, y) }); // x + 2y
            Polynomial p2 = new Polynomial(new Term[] { new Term(3.0, x), new Term(4.0, Monomial.One) }); // 3x + 4

            Polynomial sum = p1.Add(p2); // 4x + 2y + 4
            Assert.AreEqual(3, sum.Terms.Count);
            Assert.IsTrue(sum.Terms.Any(t => t.Coefficient.Equals(4.0) && t.Monomial.Equals(x)));
            Assert.IsTrue(sum.Terms.Any(t => t.Coefficient.Equals(2.0) && t.Monomial.Equals(y)));
            Assert.IsTrue(sum.Terms.Any(t => t.Coefficient.Equals(4.0) && t.Monomial.Equals(Monomial.One)));
        }

        [TestMethod]
        public void Polynomial_Add_WithZeroPolynomial_ReturnsOriginal()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p1 = new Polynomial(new Term[] { new Term(1.0, x) }); // x
            Polynomial p2 = new Polynomial(); // 0

            Polynomial sum = p1.Add(p2);
            Assert.AreEqual(p1, sum);
        }

        [TestMethod]
        public void Polynomial_Subtract_NormalCase_ReturnsDifference()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial y = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } }));

            Polynomial p1 = new Polynomial(new Term[] { new Term(5.0, x), new Term(2.0, y) }); // 5x + 2y
            Polynomial p2 = new Polynomial(new Term[] { new Term(3.0, x), new Term(1.0, Monomial.One) }); // 3x + 1

            Polynomial diff = p1.Subtract(p2); // 2x + 2y - 1
            Assert.AreEqual(3, diff.Terms.Count);
            Assert.IsTrue(diff.Terms.Any(t => t.Coefficient.Equals(2.0) && t.Monomial.Equals(x)));
            Assert.IsTrue(diff.Terms.Any(t => t.Coefficient.Equals(2.0) && t.Monomial.Equals(y)));
            Assert.IsTrue(diff.Terms.Any(t => t.Coefficient.Equals(-1.0) && t.Monomial.Equals(Monomial.One)));
        }

        [TestMethod]
        public void Polynomial_Subtract_SelfSubtract_ReturnsZeroPolynomial()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p1 = new Polynomial(new Term[] { new Term(1.0, x) }); // x
            Polynomial diff = p1.Subtract(p1);
            Assert.IsTrue(diff.IsZero);
        }

        [TestMethod]
        public void Polynomial_Multiply_ByTerm_ReturnsProduct()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial y = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 } }));
            Polynomial p = new Polynomial(new Term[] { new Term(2.0, x), new Term(3.0, y) }); // 2x + 3y

            Monomial z = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "z", 1 } }));
            Term multiplier = new Term(4.0, z); // 4z

            Polynomial product = p.Multiply(multiplier); // 8xz + 12yz

            Assert.AreEqual(2, product.Terms.Count);
            Assert.IsTrue(product.Terms.Any(t => t.Coefficient.Equals(8.0) && t.Monomial.Equals(new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "z", 1 } })))));
            Assert.IsTrue(product.Terms.Any(t => t.Coefficient.Equals(12.0) && t.Monomial.Equals(new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 1 }, { "z", 1 } })))));
        }

        [TestMethod]
        public void Polynomial_Multiply_ByScalar_ReturnsProduct()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p = new Polynomial(new Term[] { new Term(2.0, x), new Term(3.0, Monomial.One) }); // 2x + 3
            Polynomial product = p.Multiply(5.0); // 10x + 15
            Assert.AreEqual(2, product.Terms.Count);
            Assert.IsTrue(product.Terms.Any(t => t.Coefficient.Equals(10.0) && t.Monomial.Equals(x)));
            Assert.IsTrue(product.Terms.Any(t => t.Coefficient.Equals(15.0) && t.Monomial.Equals(Monomial.One)));
        }

        [TestMethod]
        public void Polynomial_Multiply_ByZeroScalar_ReturnsZeroPolynomial()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p = new Polynomial(new Term[] { new Term(2.0, x) });
            Polynomial product = p.Multiply(0.0);
            Assert.IsTrue(product.IsZero);
        }

        [TestMethod]
        public void Polynomial_Equals_SameObject_ReturnsTrue()
        {
            Polynomial p = Polynomial.CreateConstant(1.0);
            Assert.IsTrue(p.Equals(p));
        }

        [TestMethod]
        public void Polynomial_Equals_EqualContent_ReturnsTrue()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p1 = new Polynomial(new Term[] { new Term(1.0, x), new Term(2.0, Monomial.One) }); // x + 2
            Polynomial p2 = new Polynomial(new Term[] { new Term(2.0, Monomial.One), new Term(1.0, x) }); // 2 + x (different order in constructor will be normalized to same)
            Assert.IsTrue(p1.Equals(p2));
        }

        [TestMethod]
        public void Polynomial_Equals_DifferentContent_ReturnsFalse()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p1 = new Polynomial(new Term[] { new Term(1.0, x) }); // x
            Polynomial p2 = new Polynomial(new Term[] { new Term(2.0, x) }); // 2x
            Assert.IsFalse(p1.Equals(p2));
        }

        [TestMethod]
        public void Polynomial_Equals_Null_ReturnsFalse()
        {
            Polynomial p1 = Polynomial.CreateConstant(1.0);
            Assert.IsFalse(p1.Equals(null));
        }

        [TestMethod]
        public void Polynomial_GetHashCode_EqualPolynomials_ReturnSameHashCode()
        {
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Polynomial p1 = new Polynomial(new Term[] { new Term(1.0, x), new Term(2.0, Monomial.One) });
            Polynomial p2 = new Polynomial(new Term[] { new Term(2.0, Monomial.One), new Term(1.0, x) });
            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
        }

        [TestMethod]
        public void Polynomial_ToString_NormalCase_ReturnsCorrectString()
        {
            Monomial x2y = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } }));
            Monomial z = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "z", 1 } }));
            Polynomial p = new Polynomial(new Term[]
            {
                new Term(2.0, x2y),  // 2x^2y
                new Term(5.0, Monomial.One), // 5
                new Term(-3.0, z) // -3z
            });
            // Sorting is done by Monomial.CompareTo (default alphabetical variable sort)
            // The ToString uses OrderByDescending(t => t.Monomial) which uses Monomial's IComparable (total degree then variable alphabetical).
            // So, x^2y (deg 3), z (deg 1), 1 (deg 0)
            // Output order: 2x^2y - 3z + 5
            string expected = "2x^2y - 3z + 5";
            string actual = p.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Polynomial_ToString_ZeroPolynomial_ReturnsZeroString()
        {
            Polynomial p = new Polynomial();
            Assert.AreEqual("0", p.ToString());
        }
    }
}
