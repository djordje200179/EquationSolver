using System;
using System.Diagnostics;
using System.Linq;

namespace EquationSolver {
	internal class Program {
		internal void Run() {
			var sw = Stopwatch.StartNew();

			var solver = new EquationSolver(x => Math.Cos(x), (-10, 10), (ulong)1e7);
			var results = solver.FindSolutions(true).ToList();

			sw.Stop();

			Console.WriteLine($"Number of roots found: {results.Count}");
			Console.WriteLine($"Time spent: {sw.Elapsed.TotalSeconds, 0:F2} seconds");
			Console.WriteLine("------------------");

			for (var i = 0; i < results.Count; i++)
				Console.WriteLine($"{i + 1}. root: {results[i], 10:F7}");
		}

		private static void Main() {
			var program = new Program();
			program.Run();
		}
	}
}
