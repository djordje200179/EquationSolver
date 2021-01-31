using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace EquationSolver {
	internal class Program {
		internal void Run() {
			var sw = Stopwatch.StartNew();

			var solver = new EquationSolver(x => Math.Cos(x), (-10, 10), (ulong)1e7);
			var results = solver.FindSolutions(true).ToList();

			sw.Stop();

			Console.OutputEncoding = Encoding.UTF8;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("rs");
			Console.WriteLine($"Пронађено коријена: {results.Count}");
			Console.WriteLine($"Потрошено вријеме: {sw.Elapsed.TotalSeconds, 0:F2} секунди");
			Console.WriteLine("------------------");

			for (var i = 0; i < results.Count; i++)
				Console.WriteLine($"{i + 1}. коријен: {results[i], 10:F7}");

		}

		private static void Main() {
			var program = new Program();
			program.Run();
		}
	}
}
