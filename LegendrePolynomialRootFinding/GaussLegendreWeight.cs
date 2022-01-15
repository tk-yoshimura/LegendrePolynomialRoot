using MultiPrecision;

namespace LegendrePolynomialRootFinding {
    internal static class GaussLegendreWeight<N> where N : struct, IConstant {
        public static MultiPrecision<N> Eval(MultiPrecision<N> x_root, Polynomial<N> poly) {
            MultiPrecision<N> d = poly.Diff(x_root);

            MultiPrecision<N> w = 2 / ((1 - x_root * x_root) * d * d);

            return w;
        }
    }
}
