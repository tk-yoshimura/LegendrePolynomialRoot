using MultiPrecision;
using System;
using System.Collections.Generic;
using System.IO;

namespace LegendrePolynomialRootFinding {

    internal struct Plus16<N> : IConstant where N : struct, IConstant {
        public int Value => checked(default(N).Value + 16);
    }

    internal class Program {
        static void Main() {
            using StreamWriter sw = new("../../../../results_disused/roots_n64.csv");

            sw.WriteLine("x,w,Pn'(x),Pn(x+eps),Pn(x-eps)");

            MultiPrecision<Plus16<Pow2.N64>> dx = 0.25;

            for (int n = 4; n <= 128; n++) {
                Console.WriteLine($"P{n} root finding...");

                Polynomial<Plus16<Pow2.N64>> poly = PolynomialGenerator.Table<Plus16<Pow2.N64>>(n);

                List<MultiPrecision<Plus16<Pow2.N64>>> roots = new();

                while (dx > 0) {
                    roots.Clear();

                    if ((n & 1) == 1) {
                        roots.Add(0);
                    }

                    MultiPrecision<Plus16<Pow2.N64>> prev_y = poly.Value(dx / 2);

                    for (MultiPrecision<Plus16<Pow2.N64>> x = dx * 3 / 2; x < 1; x += dx) {
                        MultiPrecision<Plus16<Pow2.N64>> y = poly.Value(x);

                        if (prev_y.Sign != y.Sign) {
                            MultiPrecision<Plus16<Pow2.N64>> root = MultiPrecisionUtil.NewtonRaphsonRootFinding(
                                x - dx / 2, poly.Value, poly.Diff, x - dx, x + dx / 4, break_overshoot: false, max_iterations: 256
                            );

                            roots.Add(root);

                            Console.WriteLine($"  root found x={root}");
                        }

                        prev_y = y;
                    }

                    if (roots.Count >= (n + 1) / 2) {
                        break;
                    }

                    dx /= 2;
                    Console.WriteLine($"  reduce search width. dx={dx}");
                }

                sw.WriteLine($"P{n}");

                foreach (MultiPrecision<Plus16<Pow2.N64>> x in roots) {
                    MultiPrecision<Pow2.N64> w = GaussLegendreWeight<Plus16<Pow2.N64>>.Eval(x, poly).Convert<Pow2.N64>();
                    MultiPrecision<Pow2.N64> d = poly.Diff(x).Convert<Pow2.N64>();

                    MultiPrecision<Pow2.N64> my = poly.Value(
                        MultiPrecision<Pow2.N64>.BitDecrement(x.Convert<Pow2.N64>()).Convert<Plus16<Pow2.N64>>()
                    ).Convert<Pow2.N64>();

                    MultiPrecision<Pow2.N64> py = poly.Value(
                        MultiPrecision<Pow2.N64>.BitIncrement(x.Convert<Pow2.N64>()).Convert<Plus16<Pow2.N64>>()
                    ).Convert<Pow2.N64>();

                    sw.WriteLine($"{x.Convert<Pow2.N64>()},{w},{d},{my:e4},{py:e4}");
                }

                sw.Flush();
            }

            Console.WriteLine("END");
            Console.Read();
        }
    }
}
