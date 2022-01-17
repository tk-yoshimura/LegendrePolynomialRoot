using MultiPrecision;

namespace LegendrePolynomialRootFinding {
    internal struct Plus16<N> : IConstant where N : struct, IConstant {
        public int Value => checked(default(N).Value + 16);
    }
}
