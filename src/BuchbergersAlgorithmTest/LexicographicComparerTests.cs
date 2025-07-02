using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System;

namespace BuchbergersAlgorithmTest
{
    [TestClass]
    public sealed class LexicographicComparerTests
    {
        [TestMethod]
        public void Compare_XGreaterThanY_ReturnsPositive()
        {
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x", "y"));
            Monomial x2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } })); // x^2
            Monomial xy = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } })); // xy
            Assert.IsTrue(comparer.Compare(x2, xy) > 0);
        }

        [TestMethod]
        public void Compare_YGreaterThanX_ReturnsNegative()
        {
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("y", "x")); // y > x
            Monomial y2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 2 } })); // y^2
            Monomial xy = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } })); // xy
            Assert.IsTrue(comparer.Compare(y2, xy) > 0);
        }

        [TestMethod]
        public void Compare_SameMonomial_ReturnsZero()
        {
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x", "y"));
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Assert.AreEqual(0, comparer.Compare(m1, m2));
        }

        [TestMethod]
        public void Compare_DifferentVarOrder_ChangesResult()
        {
            Monomial x2y = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial xy2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } })); // xy^2

            IMonomialComparer comparerXY = new LexicographicComparer(ImmutableList.Create("x", "y"));
            Assert.IsTrue(comparerXY.Compare(x2y, xy2) > 0); // x^2y > xy^2 (because 2>1 for x)

            IMonomialComparer comparerYX = new LexicographicComparer(ImmutableList.Create("y", "x"));
            Assert.IsTrue(comparerYX.Compare(x2y, xy2) < 0); // x^2y < xy^2 (because 1<2 for y, then check x)
        }

        [TestMethod]
        public void Compare_OneMonomialIsOne_ReturnsCorrectly()
        {
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x"));
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Assert.IsTrue(comparer.Compare(x, Monomial.One) > 0); // x > 1
            Assert.IsTrue(comparer.Compare(Monomial.One, x) < 0); // 1 < x
            Assert.AreEqual(0, comparer.Compare(Monomial.One, Monomial.One));
        }

        [TestMethod]
        public void Compare_WithNulls_ReturnsCorrectly()
        {
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x"));
            Monomial x = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Assert.IsTrue(comparer.Compare(x, null) > 0);
            Assert.IsTrue(comparer.Compare(null, x) < 0);
            Assert.AreEqual(0, comparer.Compare(null, null));
        }

        [TestMethod]
        public void Compare_ComplexCase_LexicographicOrder()
        {
            IMonomialComparer comparer = new LexicographicComparer(ImmutableList.Create("x", "y", "z"));
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 0 }, { "z", 1 } })); // x^2z
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 3 }, { "z", 0 } })); // xy^3
            Monomial m3 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 }, { "z", 0 } })); // x^2y

            // x^2z vs xy^3: x^2 > x^1, so x^2z > xy^3
            Assert.IsTrue(comparer.Compare(m1, m2) > 0);
            // x^2z vs x^2y: x exponents are equal (2 vs 2), then y exponents (0 vs 1), so x^2z < x^2y
            Assert.IsTrue(comparer.Compare(m1, m3) < 0);
            // xy^3 vs x^2y: xy^3 < x^2y (because x^1 < x^2)
            Assert.IsTrue(comparer.Compare(m2, m3) < 0);
        }
    }
}
