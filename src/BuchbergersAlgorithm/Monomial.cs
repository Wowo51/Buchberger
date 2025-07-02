using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BuchbergersAlgorithm
{
    public sealed class Monomial : IComparable<Monomial>, IEquatable<Monomial>
    {
        private readonly ImmutableSortedDictionary<string, int> _exponents;

        public Monomial(ImmutableSortedDictionary<string, int> exponents)
        {
            // Filter out variables with exponent 0 as they don't contribute to the monomial
            _exponents = ImmutableSortedDictionary.CreateRange(exponents.Where(e => e.Value > 0));
        }

        public ImmutableSortedDictionary<string, int> Exponents
        {
            get { return _exponents; }
        }

        public static Monomial One
        {
            get { return new Monomial(ImmutableSortedDictionary<string, int>.Empty); }
        }

        public int GetExponent(string variable)
        {
            if (_exponents.TryGetValue(variable, out int value))
            {
                return value;
            }
            return 0;
        }

        public int GeTotalDegree()
        {
            return _exponents.Sum(e => e.Value);
        }

        public Monomial Multiply(Monomial other)
        {
            Dictionary<string, int> newExponents = _exponents.ToDictionary(e => e.Key, e => e.Value);

            foreach (KeyValuePair<string, int> entry in other._exponents)
            {
                if (newExponents.ContainsKey(entry.Key))
                {
                    newExponents[entry.Key] += entry.Value;
                }
                else
                {
                    newExponents.Add(entry.Key, entry.Value);
                }
            }
            return new Monomial(ImmutableSortedDictionary.CreateRange(newExponents));
        }

        // Divides 'this' monomial by 'other'. Returns null if not divisible.
        public Monomial? Divide(Monomial other)
        {
            Dictionary<string, int> newExponents = _exponents.ToDictionary(e => e.Key, e => e.Value);

            foreach (KeyValuePair<string, int> entry in other._exponents)
            {
                if (!newExponents.ContainsKey(entry.Key) || newExponents[entry.Key] < entry.Value)
                {
                    return null; // Not divisible
                }
                newExponents[entry.Key] -= entry.Value;
            }
            return new Monomial(ImmutableSortedDictionary.CreateRange(newExponents));
        }
        
        // Static method to compute LCM of two monomials
        public static Monomial Lcm(Monomial m1, Monomial m2)
        {
            Dictionary<string, int> lcmExponents = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> entry in m1.Exponents)
            {
                lcmExponents[entry.Key] = entry.Value;
            }

            foreach (KeyValuePair<string, int> entry in m2.Exponents)
            {
                if (lcmExponents.ContainsKey(entry.Key))
                {
                    lcmExponents[entry.Key] = Math.Max(lcmExponents[entry.Key], entry.Value);
                }
                else
                {
                    lcmExponents.Add(entry.Key, entry.Value);
                }
            }
            return new Monomial(ImmutableSortedDictionary.CreateRange(lcmExponents));
        }

        // Implement IEquatable<Monomial>
        public bool Equals(Monomial? other)
        {
            if (other is null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
            return _exponents.SequenceEqual(other._exponents);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Monomial);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (KeyValuePair<string, int> entry in _exponents)
            {
                hash = HashCode.Combine(hash, entry.Key.GetHashCode(), entry.Value.GetHashCode());
            }
            return hash;
        }

        // This IComparable is primarily for sorting within collections for consistency,
        // but actual monomial ordering (lex, grevlex) will use IMonomialComparer.
        public int CompareTo(Monomial? other)
        {
            if (other is null)
            {
                return 1;
            }

            // Compare by total degree first (common for many orders)
            int degreeComparison = GeTotalDegree().CompareTo(other.GeTotalDegree());
            if (degreeComparison != 0)
            {
                return degreeComparison;
            }

            // For deterministic comparison, iterate through sorted variable names
            List<string> allVars = _exponents.Keys.Union(other._exponents.Keys).OrderBy(v => v).ToList();
            foreach (string var in allVars)
            {
                int thisExp = GetExponent(var);
                int otherExp = other.GetExponent(var);
                int expComparison = thisExp.CompareTo(otherExp);
                if (expComparison != 0)
                {
                    return expComparison;
                }
            }
            return 0; // Monomials are equal
        }

        public override string ToString()
        {
            if (_exponents.Count == 0)
            {
                return "1";
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<string, int> entry in _exponents)
            {
                sb.Append(entry.Key);
                if (entry.Value > 1)
                {
                    sb.Append("^");
                    sb.Append(entry.Value);
                }
            }
            return sb.ToString();
        }
    }
}
