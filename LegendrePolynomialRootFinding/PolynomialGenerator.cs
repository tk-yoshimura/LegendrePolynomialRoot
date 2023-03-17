using MultiPrecision;

namespace LegendrePolynomialRootFinding {
    internal static class PolynomialGenerator {
        static readonly List<Fraction[]> coef_table = new();

        static PolynomialGenerator() {
            coef_table.Add(new Fraction[] { 1 });
            coef_table.Add(new Fraction[] { 0, 1 });
        }

        public static Polynomial<N> Table<N>(int n) where N : struct, IConstant {
            if (n < coef_table.Count) {
                return new Polynomial<N>(coef_table[n].Select(f => f.ToMultiPrecision<N>()).ToArray());
            }

            for (int i = coef_table.Count; i <= n; i++) {
                Fraction[] coef = new Fraction[i + 1];

                coef[0] = -coef_table[i - 2][0] * (i - 1) / i;

                for (int k = 1; k <= i - 2; k++) {
                    coef[k] = coef_table[i - 1][k - 1] * (2 * i - 1) / i - coef_table[i - 2][k] * (i - 1) / i;
                }

                coef[^2] = 0;
                coef[^1] = coef_table[i - 1][^1] * (2 * i - 1) / i;

                coef_table.Add(coef);
            }

            return new Polynomial<N>(coef_table[n].Select(f => f.ToMultiPrecision<N>()).ToArray());
        }
    }
}
