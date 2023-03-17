using MultiPrecision;

namespace LegendrePolynomialRootFinding {

    internal class Program_RootCheck {
        static void Main() {
            using BinaryReader sbr = new(File.OpenRead("../../../../results_disused/roots_n64_01.bin"));

            for (int n = 4; n <= 255; n++) {
                int ns = sbr.ReadInt32();

                for (int i = 0; i < n; i++) {
                    sbr.ReadMultiPrecision<Pow2.N64>();
                    sbr.ReadMultiPrecision<Pow2.N64>();
                }
            }

            sbr.ReadInt32();

            Polynomial<Pow2.N128> poly = PolynomialGenerator.Table<Pow2.N128>(256);

            for (int i = 0; i < 256; i++) {
                Console.WriteLine(i);

                MultiPrecision<Pow2.N64> x = sbr.ReadMultiPrecision<Pow2.N64>() * 2 - 1;

                MultiPrecision<Pow2.N64> xm = poly.Value(MultiPrecision<Pow2.N64>.BitDecrement(x).Convert<Pow2.N128>()).Convert<Pow2.N64>();
                MultiPrecision<Pow2.N64> x0 = poly.Value(x.Convert<Pow2.N128>()).Convert<Pow2.N64>();
                MultiPrecision<Pow2.N64> xp = poly.Value(MultiPrecision<Pow2.N64>.BitIncrement(x).Convert<Pow2.N128>()).Convert<Pow2.N64>();

                Console.WriteLine($"{xm:e4}");
                Console.WriteLine($"{x0:e4}");
                Console.WriteLine($"{xp:e4}");

                sbr.ReadMultiPrecision<Pow2.N64>();
            }

            Console.WriteLine("END");
            Console.Read();
        }
    }
}
