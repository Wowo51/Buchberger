using BuchbergersAlgorithm;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace BuchbergersAlgorithm
{
    public static class PolynomialOperations
    {
        // Calculate the S-polynomial of two polynomials f and g.
        public static Polynomial CalculateSPolynomial(Polynomial f, Polynomial g, IMonomialComparer comparer)
        {
            if (f.IsZero || g.IsZero)
            {
                return new Polynomial(); // S-polynomial involving a zero polynomial is zero.
            }

            Term ltF = f.GetLeadingTerm(comparer);
            Term ltG = g.GetLeadingTerm(comparer);

            Monomial lcmMonomial = Monomial.Lcm(ltF.Monomial, ltG.Monomial);

            // Calculate a_f = LCM(LM(f), LM(g)) / LT(f)
            Monomial? quotientF = lcmMonomial.Divide(ltF.Monomial);
            // This quotient should not be null if LCM is correctly computed and contains both monomials' variables/exponents.
            if (quotientF is null) { return new Polynomial(); } 

            // Calculate a_g = LCM(LM(f), LM(g)) / LT(g)
            Monomial? quotientG = lcmMonomial.Divide(ltG.Monomial);
            if (quotientG is null) { return new Polynomial(); }

            // Terms to multiply f and g by.
            // S_poly = (lcm(LM(f),LM(g))/LT(f)) * f - (lcm(LM(f),LM(g))/LT(g)) * g
            // Note: LT(f) = Coeff(f)*LM(f)
            // So we need (lcm_monomial / LM(f)) * (1/Coeff(f)) as the multiplicative term for f
            Term multiplyTermF = new Term(1.0 / ltF.Coefficient, quotientF);

            // Same for g
            Term multiplyTermG = new Term(1.0 / ltG.Coefficient, quotientG);

            Polynomial term1 = f.Multiply(multiplyTermF);
            Polynomial term2 = g.Multiply(multiplyTermG);

            return term1.Subtract(term2);
        }

        // Multivariate division algorithm (reduces f by G until irreducible)
        public static Polynomial Reduce(Polynomial f, ImmutableList<Polynomial> G, IMonomialComparer comparer)
        {
            Polynomial remainder = f;
            bool reductionOccurred = true;

            while (remainder.IsZero == false && reductionOccurred)
            {
                reductionOccurred = false;
                Term ltRemainder = remainder.GetLeadingTerm(comparer); 

                foreach (Polynomial g in G)
                {
                    if (g.IsZero)
                    {
                        continue;
                    }

                    Term ltG = g.GetLeadingTerm(comparer);
                    Monomial? quotientMonomial = ltRemainder.Monomial.Divide(ltG.Monomial);

                    // If LM(remainder) is divisible by LM(g)
                    if (quotientMonomial != null) 
                    {
                        double lcRemainder = ltRemainder.Coefficient;
                        double lcG = ltG.Coefficient;

                        // Term to multiply g by for reduction
                        Term reductionTerm = new Term(lcRemainder / lcG, quotientMonomial);

                        Polynomial product = g.Multiply(reductionTerm);
                        remainder = remainder.Subtract(product);
                        
                        reductionOccurred = true;
                        // A reduction occurred, restart the outer while loop to re-evaluate leading term of remainder
                        // and attempt further reductions.
                        break; 
                    }
                }
            }
            return remainder;
        }

        // Makes a polynomial monic, i.e., sets its leading coefficient to 1.
        public static Polynomial MakeMonic(Polynomial p, IMonomialComparer comparer)
        {
            if (p.IsZero)
            {
                return p;
            }

            double lc = p.GetLeadingCoefficient(comparer);
            
            // Use epsilon for checking against zero to account for floating point inaccuracies.
            if (System.Math.Abs(lc) < double.Epsilon) 
            {
                // If leading coefficient is effectively zero, the polynomial is considered zero.
                // This can happen if coefficients cancel out due to floating point math creating a near-zero LC.
                return new Polynomial(); 
            }

            // If already monic (or very close to 1), return as is.
            if (System.Math.Abs(lc - 1.0) < double.Epsilon) 
            {
                return p;
            }
            
            return p.Multiply(1.0 / lc);
        }
    }
}
