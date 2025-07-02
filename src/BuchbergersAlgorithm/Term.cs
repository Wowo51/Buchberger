using System;
using System.Collections.Generic;
using System.Text; // Added for StringBuilder

namespace BuchbergersAlgorithm
{
    public sealed class Term : IEquatable<Term>
    {
        private readonly double _coefficient;
        private readonly Monomial _monomial;
        public Term(double coefficient, Monomial monomial)
        {
            _coefficient = coefficient;
            _monomial = monomial ?? Monomial.One; // Ensure monomial is not null
        }

        public double Coefficient
        {
            get
            {
                return _coefficient;
            }
        }

        public Monomial Monomial
        {
            get
            {
                return _monomial;
            }
        }

        public static Term Zero
        {
            get
            {
                return new Term(0.0, Monomial.One);
            }
        }

        public Term Multiply(double scalar)
        {
            return new Term(_coefficient * scalar, _monomial);
        }

        public Term Multiply(Monomial otherMonomial)
        {
            return new Term(_coefficient, _monomial.Multiply(otherMonomial));
        }

        public Term Multiply(Term otherTerm)
        {
            return new Term(_coefficient * otherTerm._coefficient, _monomial.Multiply(otherTerm._monomial));
        }

        // ToString() returns the standard algebraic string representation of the term.
        // This includes its sign and handles coefficients of 1 or -1 for monomials.
        public override string ToString()
        {
            if (System.Math.Abs(Coefficient) < double.Epsilon) // Check if coefficient is effectively zero
            {
                return "0";
            }

            StringBuilder sb = new StringBuilder();
            // Determines the string representation of the term, including its sign and handling for coefficients of 1 and -1.
            if (Monomial.Equals(Monomial.One)) // This is a constant term (e.g., 5, -1, 0.5)
            {
                sb.Append(_coefficient.ToString()); // Directly append coefficient with its sign
            }
            else // Term has a monomial (e.g., x, 2x, -x)
            {
                if (System.Math.Abs(_coefficient - 1.0) < double.Epsilon) // Coefficient is 1 (e.g., "x")
                {
                // No coefficient "1" is appended
                }
                else if (System.Math.Abs(_coefficient + 1.0) < double.Epsilon) // Coefficient is -1 (e.g., "-x")
                {
                    sb.Append("-"); // Only "-" is appended
                }
                else // General case: coefficient is not 0, 1, or -1 (e.g., 2x, -2x)
                {
                    sb.Append(_coefficient.ToString()); // Append coefficient with its sign
                }

                sb.Append(Monomial.ToString()); // Append the monomial
            }

            return sb.ToString();
        }

        // New method: Returns the string representation of the term's magnitude (absolute coefficient, no sign if implied)
        public string ToMagnitudeString()
        {
            if (System.Math.Abs(Coefficient) < double.Epsilon)
            {
                return "0";
            }

            StringBuilder sb = new StringBuilder();
            double absCoefficient = Math.Abs(_coefficient);

            if (Monomial.Equals(Monomial.One))
            {
                sb.Append(absCoefficient.ToString());
            }
            else
            {
                if (System.Math.Abs(absCoefficient - 1.0) < double.Epsilon)
                {
                    // No coefficient "1" is appended for monomial terms.
                }
                else
                {
                    sb.Append(absCoefficient.ToString());
                }
                sb.Append(Monomial.ToString());
            }
            return sb.ToString();
        }

        // Implement IEquatable<Term>
        public bool Equals(Term? other)
        {
            if (other is null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // Use a small epsilon for double comparison to account for floating point inaccuracies
            return Math.Abs(_coefficient - other._coefficient) < double.Epsilon && _monomial.Equals(other._monomial);
        }

        public override int GetHashCode()
        {
            // Compute hash code combining rounded coefficient and monomial hash.
            // Rounding coefficient to a certain precision before hashing to ensure consistent hashes
            // for floating-point numbers that are mathematically equal but slightly different binary representations.
            long coefficientHash = BitConverter.DoubleToInt64Bits(Math.Round(_coefficient, 6)); // Round to 6 decimal places
            return HashCode.Combine(coefficientHash, _monomial.GetHashCode());
        }
    }
}