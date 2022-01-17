using MultiPrecision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LegendrePolynomialRootFinding {

    internal class Program01 {
        static void Main() {
            using StreamWriter sw = new("../../../../results/roots_n4_01.csv");

            sw.WriteLine("x,w");

            MultiPrecision<Plus16<Pow2.N4>> dx = 0.25;

            for (int n = 4; n <= 128; n++) {
                Console.WriteLine($"P{n} root finding...");

                Polynomial<Plus16<Pow2.N4>> poly = PolynomialGenerator.Table<Plus16<Pow2.N4>>(n);

                List<MultiPrecision<Plus16<Pow2.N4>>> roots = new();

                while (dx > 0) {
                    roots.Clear();

                    if ((n & 1) == 1) {
                        roots.Add(0);
                    }

                    MultiPrecision<Plus16<Pow2.N4>> prev_y = poly.Value(dx / 2);

                    for (MultiPrecision<Plus16<Pow2.N4>> x = dx * 3 / 2; x < 1; x += dx) {
                        MultiPrecision<Plus16<Pow2.N4>> y = poly.Value(x);

                        if (prev_y.Sign != y.Sign) {
                            MultiPrecision<Plus16<Pow2.N4>> root = MultiPrecisionUtil.NewtonRaphsonRootFinding(
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


                List<(MultiPrecision<Pow2.N4> x, MultiPrecision<Pow2.N4> w)> pts = new();

                foreach (MultiPrecision<Plus16<Pow2.N4>> x in roots) {
                    MultiPrecision<Pow2.N4> w = GaussLegendreWeight<Plus16<Pow2.N4>>.Eval(x, poly).Convert<Pow2.N4>() / 2;

                    if (x != 0) {
                        MultiPrecision<Pow2.N4> x_msft = ((1 - x) / 2).Convert<Pow2.N4>();
                        MultiPrecision<Pow2.N4> x_psft = ((x + 1) / 2).Convert<Pow2.N4>();

                        pts.Add((x_msft, w));
                        pts.Add((x_psft, w));
                    }
                    else { 
                        pts.Add((MultiPrecision<Pow2.N4>.Point5, w));
                    }
                }

                pts = pts.OrderBy((v) => v.x).ToList();

                sw.WriteLine($"P{n},");

                foreach ((var x, var w) in pts) {
                    sw.WriteLine($"{x},{w}");
                }

                sw.Flush();
            }

            Console.WriteLine("END");
            Console.Read();
        }
    }
}
