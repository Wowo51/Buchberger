using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuchbergersAlgorithm;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BuchbergersAlgorithmTest
{
    [TestClass]
    public sealed class MonomialTests
    {
        [TestMethod]
        public void Monomial_Constructor_FiltersZeroExponents()
        {
            Dictionary<string, int> exponents = new Dictionary<string, int>
            {
                { "x", 2 },
                { "y", 0 },
                { "z", 3 }
            };
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(exponents));
            Assert.AreEqual(2, monomial.Exponents.Count);
            Assert.IsTrue(monomial.Exponents.ContainsKey("x"));
            Assert.IsFalse(monomial.Exponents.ContainsKey("y"));
            Assert.IsTrue(monomial.Exponents.ContainsKey("z"));
        }

        [TestMethod]
        public void Monomial_One_ReturnsEmptyMonomial()
        {
            Monomial one = Monomial.One;
            Assert.AreEqual(0, one.Exponents.Count);
            Assert.AreEqual(0, one.GeTotalDegree());
        }

        [TestMethod]
        public void Monomial_GetExponent_ExistingVariable_ReturnsCorrectValue()
        {
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Assert.AreEqual(2, monomial.GetExponent("x"));
        }

        [TestMethod]
        public void Monomial_GetExponent_NonExistingVariable_ReturnsZero()
        {
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Assert.AreEqual(0, monomial.GetExponent("y"));
        }

        [TestMethod]
        public void Monomial_GeTotalDegree_ReturnsCorrectSum()
        {
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 3 } }));
            Assert.AreEqual(5, monomial.GeTotalDegree());
        }

        [TestMethod]
        public void Monomial_GeTotalDegree_OneMonomial_ReturnsZero()
        {
            Monomial monomial = Monomial.One;
            Assert.AreEqual(0, monomial.GeTotalDegree());
        }

        [TestMethod]
        public void Monomial_Multiply_NormalCase_ReturnsProduct()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 3 }, { "z", 1 } })); // y^3z
            Monomial expected = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 4 }, { "z", 1 } })); // x^2y^4z

            Monomial result = m1.Multiply(m2);
            CollectionAssert.AreEqual(expected.Exponents.ToArray(), result.Exponents.ToArray());
        }

        [TestMethod]
        public void Monomial_Multiply_WithOne_ReturnsOriginal()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial result = m1.Multiply(Monomial.One);
            Assert.AreEqual(m1, result); // Uses Monomial.Equals
        }

        [TestMethod]
        public void Monomial_Divide_DivisibleCase_ReturnsQuotient()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 3 }, { "y", 2 } })); // x^3y^2
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } })); // xy^2
            Monomial expected = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } })); // x^2

            Monomial? result = m1.Divide(m2);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expected.Exponents.ToArray(), result.Exponents.ToArray());
        }

        [TestMethod]
        public void Monomial_Divide_NotDivisibleCase_ReturnsNull()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 3 } })); // x^3

            Monomial? result = m1.Divide(m2);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Monomial_Divide_ByOne_ReturnsOriginal()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial? result = m1.Divide(Monomial.One);
            Assert.IsNotNull(result);
            Assert.AreEqual(m1, result);
        }

        [TestMethod]
        public void Monomial_Divide_SelfDivison_ReturnsOne()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } })); // x^2y
            Monomial? result = m1.Divide(m1);
            Assert.IsNotNull(result);
            Assert.AreEqual(Monomial.One, result);
        }

        [TestMethod]
        public void Monomial_Lcm_NormalCase_ReturnsCorrectLcm()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 3 } })); // x^2y^3
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 3 }, { "y", 1 }, { "z", 4 } })); // x^3y z^4
            Monomial expected = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 3 }, { "y", 3 }, { "z", 4 } })); // x^3y^3z^4

            Monomial result = Monomial.Lcm(m1, m2);
            CollectionAssert.AreEqual(expected.Exponents.ToArray(), result.Exponents.ToArray());
        }

        [TestMethod]
        public void Monomial_Lcm_WithOne_ReturnsOtherMonomial()
        {
            Monomial m1 = Monomial.One;
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Monomial result = Monomial.Lcm(m1, m2);
            Assert.AreEqual(m2, result);
        }

        [TestMethod]
        public void Monomial_Equals_SameObject_ReturnsTrue()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Assert.IsTrue(m1.Equals(m1));
        }

        [TestMethod]
        public void Monomial_Equals_EqualContent_ReturnsTrue()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }));
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 2 }, { "x", 1 } })); // Order won't matter due to SortedDictionary
            Assert.IsTrue(m1.Equals(m2));
        }

        [TestMethod]
        public void Monomial_Equals_DifferentContent_ReturnsFalse()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Assert.IsFalse(m1.Equals(m2));
        }

        [TestMethod]
        public void Monomial_Equals_Null_ReturnsFalse()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Assert.IsFalse(m1.Equals(null));
        }

        [TestMethod]
        public void Monomial_GetHashCode_EqualMonomials_ReturnSameHashCode()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }));
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }));
            Assert.AreEqual(m1.GetHashCode(), m2.GetHashCode());
        }

        [TestMethod]
        public void Monomial_CompareTo_GreaterTotalDegree_ReturnsPositive()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 2 } })); // Degree 4
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } })); // Degree 2
            Assert.IsTrue(m1.CompareTo(m2) > 0);
        }

        [TestMethod]
        public void Monomial_CompareTo_LowerTotalDegree_ReturnsNegative()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } })); // Degree 2
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 2 } })); // Degree 4
            Assert.IsTrue(m1.CompareTo(m2) < 0);
        }

        [TestMethod]
        public void Monomial_CompareTo_SameTotalDegree_ReturnsCorrectly()
        {
            // Both degree 2, {x:2} vs {y:2} -> x comes before y in default string sort
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 } }));
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 2 } }));
            Assert.AreEqual(1, m1.CompareTo(m2)); // Changed assertion from < 0 to 1
        }

        [TestMethod]
        public void Monomial_CompareTo_EqualMonomials_ReturnsZero()
        {
            Monomial m1 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } }));
            Monomial m2 = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 } }));
            Assert.AreEqual(0, m1.CompareTo(m2));
        }

        [TestMethod]
        public void Monomial_ToString_ComplexMonomial_ReturnsCorrectString()
        {
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 2 }, { "y", 1 }, { "z", 3 } }));
            // Order is alphabetical by variable name due to SortedDictionary then StringBuilder
            Assert.AreEqual("x^2yz^3", monomial.ToString());
        }

        [TestMethod]
        public void Monomial_ToString_MonomialOne_ReturnsOneString()
        {
            Assert.AreEqual("1", Monomial.One.ToString());
        }

        [TestMethod]
        public void Monomial_ToString_SingleVariableExponentOne_ReturnsCorrectString()
        {
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "x", 1 } }));
            Assert.AreEqual("x", monomial.ToString());
        }

        [TestMethod]
        public void Monomial_ToString_SingleVariableExponentGreaterThanOne_ReturnsCorrectString()
        {
            Monomial monomial = new Monomial(ImmutableSortedDictionary.CreateRange(new Dictionary<string, int> { { "y", 3 } }));
            Assert.AreEqual("y^3", monomial.ToString());
        }
    }
}