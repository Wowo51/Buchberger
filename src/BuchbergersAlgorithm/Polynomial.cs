using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text; // Added for StringBuilder

namespace BuchbergersAlgorithm
{
    public sealed class Polynomial : IEquatable<Polynomial>
    {
        private ImmutableList<Term> _terms;

        public Polynomial(IEnumerable<Term> terms)
        {
            _terms = NormalizeTerms(terms);
        }

        public Polynomial()
        {
            _terms = ImmutableList<Term>.Empty;
        }

        public ImmutableList<Term> Terms
        {
            get { return _terms; }
        }

        public bool IsZero
        {
            get { return _terms.Count == 0; }
        }

        // Static factory method for a constant polynomial
        public static Polynomial CreateConstant(double constant)
        {
            if (constant == 0.0)
            {
                return new Polynomial();
            }
            return new Polynomial(new Term[] { new Term(constant, Monomial.One) });
        }

        // NormalizeTerms: combine like terms and remove terms with zero coefficients
        private ImmutableList<Term> NormalizeTerms(IEnumerable<Term> terms)
        {
            Dictionary<Monomial, double> combinedCoefficients = new Dictionary<Monomial, double>();

            foreach (Term term in terms)
            {
                if (System.Math.Abs(term.Coefficient) < double.Epsilon) // Use epsilon for floating point comparison with zero
                {
                    continue;
                }

                if (combinedCoefficients.ContainsKey(term.Monomial))
                {
                    combinedCoefficients[term.Monomial] += term.Coefficient;
                }
                else
                {
                    combinedCoefficients.Add(term.Monomial, term.Coefficient);
                }
            }

            List<Term> normalizedTerms = new List<Term>();
            foreach (KeyValuePair<Monomial, double> entry in combinedCoefficients)
            {
                if (System.Math.Abs(entry.Value) >= double.Epsilon) // Filter out terms that became zero after combination
                {
                    normalizedTerms.Add(new Term(entry.Value, entry.Key));
                }
            }

            // Ensure terms are always sorted canonically (descending order by Monomial, matching ToString's expectation)
            normalizedTerms.Sort((Term t1, Term t2) => t2.Monomial.CompareTo(t1.Monomial));

            return ImmutableList.CreateRange(normalizedTerms);
        }

        public Term GetLeadingTerm(IMonomialComparer comparer)
        {
            if (IsZero)
            {
                return Term.Zero; // Conventionally, leading term of zero polynomial is zero itself.
            }
            // Find the leading term based on the provided comparer (highest monomial)
            return _terms.OrderByDescending(t => t.Monomial, comparer).First();
        }
        
        public Monomial GetLeadingMonomial(IMonomialComparer comparer)
        {
             if (IsZero)
            {
                return Monomial.One; // Convention: leading monomial of zero polynomial
            }
            return GetLeadingTerm(comparer).Monomial;
        }

        public double GetLeadingCoefficient(IMonomialComparer comparer)
        {
            if (IsZero)
            {
                return 0.0;
            }
            return GetLeadingTerm(comparer).Coefficient;
        }

        public Polynomial Add(Polynomial other)
        {
            List<Term> newTerms = new List<Term>(this._terms);
            newTerms.AddRange(other._terms);
            return new Polynomial(newTerms);
        }

        public Polynomial Subtract(Polynomial other)
        {
            List<Term> newTerms = new List<Term>(this._terms);
            foreach (Term term in other._terms)
            {
                newTerms.Add(new Term(-term.Coefficient, term.Monomial));
            }
            return new Polynomial(newTerms);
        }

        public Polynomial Multiply(Term term)
        {
            List<Term> newTerms = new List<Term>();
            foreach (Term t in _terms)
            {
                newTerms.Add(t.Multiply(term));
            }
            return new Polynomial(newTerms);
        }

        public Polynomial Multiply(double scalar)
        {
            List<Term> newTerms = new List<Term>();
            foreach (Term t in _terms)
            {
                newTerms.Add(t.Multiply(scalar));
            }
            return new Polynomial(newTerms);
        }

        public override string ToString()
        {
            if (IsZero)
            {
                return "0";
            }
            StringBuilder sb = new StringBuilder();
            // Terms are already sorted canonically by Monomial.CompareTo (descending) by NormalizeTerms.
            // Using the already sorted _terms property for consistency.
            ImmutableList<Term> sortedTerms = _terms; 

            bool firstTermPrinted = false;
            foreach (Term term in sortedTerms)
            {
                if (System.Math.Abs(term.Coefficient) < double.Epsilon) // Skip terms with effectively zero coefficient
                {
                    continue;
                }

                if (!firstTermPrinted)
                {
                    // The first term should include its own sign if negative (e.g., "-3z" or "2x^2y").
                    // Term.ToString() already includes the sign for the first term.
                    sb.Append(term.ToString()); 
                    firstTermPrinted = true;
                }
                else
                {
                    // For subsequent terms: determine spacing and " + " or " - " sign based on coefficient.
                    if (term.Coefficient > 0)
                    {
                        sb.Append(" + ");
                    }
                    else // term.Coefficient < 0
                    {
                        sb.Append(" - ");
                    }
                    // Append only the magnitude part of the term (e.g., "3z", "5").
                    sb.Append(term.ToMagnitudeString());
                }
            }
            // Trim to remove any leading/trailing spaces, although with correct logic this should not be needed.
            return sb.ToString().Trim(); 
        }

        // Implement IEquatable<Polynomial>
        public bool Equals(Polynomial? other)
        {
            if (other is null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // To compare equality, we need to compare their normalized term sets.
            // Since _terms are normalized and canonically sorted by NormalizeTerms,
            // we can directly compare their contents using SequenceEqual.
            return this._terms.SequenceEqual(other._terms);
        }

        public override int GetHashCode()
        {
            // For consistent hashing, use the already canonically ordered _terms.
            int hash = 0;
            foreach (Term term in _terms) 
            {
                hash = HashCode.Combine(hash, term.GetHashCode());
            }
            return hash;
        }
    }
}