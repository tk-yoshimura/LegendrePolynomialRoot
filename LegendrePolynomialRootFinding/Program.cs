using MultiPrecision;

namespace LegendrePolynomialRootFinding {

    internal class Program {
        static void Main_() {
            using StreamWriter sw = new("../../../../results_disused/roots_n32_01_odd.csv");

            sw.WriteLine("x,w,Pn'(x),Pn(x+eps),Pn(x-eps)");

            MultiPrecision<Plus32<Pow2.N32>> dx = 0.25;

            for (int n = 129; n <= 255; n += 2) {
                Console.WriteLine($"P{n} root finding...");

                Polynomial<Plus32<Pow2.N32>> poly = PolynomialGenerator.Table<Plus32<Pow2.N32>>(n);

                List<MultiPrecision<Plus32<Pow2.N32>>> roots = new();

                while (dx > 0) {
                    roots.Clear();

                    if ((n & 1) == 1) {
                        roots.Add(0);
                    }

                    MultiPrecision<Plus32<Pow2.N32>> prev_y = poly.Value(dx / 2);

                    for (MultiPrecision<Plus32<Pow2.N32>> x = dx * 3 / 2; x < 1; x += dx) {
                        MultiPrecision<Plus32<Pow2.N32>> y = poly.Value(x);

                        if (prev_y.Sign != y.Sign) {
                            MultiPrecision<Plus32<Pow2.N32>> root = MultiPrecisionUtil.NewtonRaphsonRootFinding(
                                x - dx / 2, poly.Value, poly.Diff, x - dx, x + dx / 4, break_overshoot: false, max_iterations: 256
                            );

                            roots.Add(root);

                            Console.WriteLine($"  root found x={root}");
                        }

                        prev_y = y;
                    }

                    if (roots.Where(root => root.IsFinite).Count() >= (n + 1) / 2) {
                        break;
                    }

                    dx /= 2;
                    Console.WriteLine($"  reduce search width. dx={dx}");
                }

                sw.WriteLine($"P{n}");

                foreach (MultiPrecision<Plus32<Pow2.N32>> x in roots) {
                    MultiPrecision<Pow2.N32> w = GaussLegendreWeight<Plus32<Pow2.N32>>.Eval(x, poly).Convert<Pow2.N32>();
                    MultiPrecision<Pow2.N32> d = poly.Diff(x).Convert<Pow2.N32>();

                    MultiPrecision<Pow2.N32> my = poly.Value(
                        MultiPrecision<Pow2.N32>.BitDecrement(x.Convert<Pow2.N32>()).Convert<Plus32<Pow2.N32>>()
                    ).Convert<Pow2.N32>();

                    MultiPrecision<Pow2.N32> py = poly.Value(
                        MultiPrecision<Pow2.N32>.BitIncrement(x.Convert<Pow2.N32>()).Convert<Plus32<Pow2.N32>>()
                    ).Convert<Pow2.N32>();

                    sw.WriteLine($"{x.Convert<Pow2.N32>()},{w},{d},{my:e4},{py:e4}");
                }

                sw.Flush();
            }

            Console.WriteLine("END");
            Console.Read();
        }
    }
}
