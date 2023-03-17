using MultiPrecision;

namespace LegendrePolynomialRootFinding {

    internal class Program01 {
        static void Main_() {
            using StreamWriter sw = new("../../../../results_disused/roots_n64_01_2.csv");
            using BinaryWriter sbw = new(File.Open("../../../../results_disused/roots_n64_01.bin", FileMode.Create));

            sw.WriteLine("x,w");

            MultiPrecision<Plus32<Pow2.N64>> dx = 0.25;

            for (int n = 4; n <= 256; n++) {
                Console.WriteLine($"P{n} root finding...");

                Polynomial<Plus32<Pow2.N64>> poly = PolynomialGenerator.Table<Plus32<Pow2.N64>>(n);

                List<MultiPrecision<Plus32<Pow2.N64>>> roots = new();

                while (dx > 0) {
                    roots.Clear();

                    if ((n & 1) == 1) {
                        roots.Add(0);
                    }

                    MultiPrecision<Plus32<Pow2.N64>> prev_y = poly.Value(dx / 2);

                    for (MultiPrecision<Plus32<Pow2.N64>> x = dx * 3 / 2; x < 1; x += dx) {
                        MultiPrecision<Plus32<Pow2.N64>> y = poly.Value(x);

                        if (prev_y.Sign != y.Sign) {
                            MultiPrecision<Plus32<Pow2.N64>> root = MultiPrecisionUtil.NewtonRaphsonRootFinding(
                                x - dx / 2, poly.Value, poly.Diff, x - dx, x + dx / 4, break_overshoot: false, max_iterations: 256
                            );

                            roots.Add(root);

                            Console.WriteLine($"  root found x={root:e10}");
                        }

                        prev_y = y;
                    }

                    if (roots.Where(root => root.IsFinite).Count() >= (n + 1) / 2) {
                        break;
                    }

                    dx /= 2;
                    Console.WriteLine($"  reduce search width. dx={dx}");
                }


                List<(MultiPrecision<Pow2.N64> x, MultiPrecision<Pow2.N64> w)> pts = new();

                foreach (MultiPrecision<Plus32<Pow2.N64>> x in roots) {
                    MultiPrecision<Pow2.N64> w = GaussLegendreWeight<Plus32<Pow2.N64>>.Eval(x, poly).Convert<Pow2.N64>() / 2;

                    if (x != 0) {
                        MultiPrecision<Pow2.N64> x_msft = ((1 - x) / 2).Convert<Pow2.N64>();
                        MultiPrecision<Pow2.N64> x_psft = ((x + 1) / 2).Convert<Pow2.N64>();

                        pts.Add((x_msft, w));
                        pts.Add((x_psft, w));
                    }
                    else {
                        pts.Add((MultiPrecision<Pow2.N64>.Point5, w));
                    }
                }

                pts = pts.OrderBy((v) => v.x).ToList();

                sw.WriteLine($"P{n},");

                sbw.Write(n);

                foreach ((var x, var w) in pts) {
                    sw.WriteLine($"{x},{w}");
                    sbw.Write(x);
                    sbw.Write(w);
                }

                sw.Flush();
                sbw.Flush();
            }

            Console.WriteLine("END");
            Console.Read();
        }
    }
}
