using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EquationSolver {
	public class EquationSolver {
		private const int NumOfSegments = 3;

		private readonly Func<double, double> _function;
		private readonly (double, double) _range;
		private readonly ulong _steps;

		public EquationSolver(Func<double, double> function, (double, double) range, ulong steps = (ulong)1e7) {
			_function = function;
			_range = range;
			_steps = steps;
		}

		public IEnumerable<double> FindSolutions(bool sort = false) {
			var step = (_range.Item2 - _range.Item1) / NumOfSegments;

			var bounds = new List<(double, double)>();

			for (var i = 0; i < NumOfSegments; i++) {
				var a = _range.Item1 + (i * step);
				var b = a + step;

				bounds.Add((a, b));
			}

			var roots = new List<double>();

			Parallel.ForEach(bounds, bound => {
				var pairs = FindPairs(bound);
				var newRoots = from pair in pairs
							   select Bisection(pair);

				roots.AddRange(newRoots);
			});

			if (sort)
				roots.Sort();

			return roots;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool DiffSign(double num1, double num2) => num1 * num2 < 0;

		private IList<(double, double)> FindPairs((double, double) limitedBounds) {
			var pairs = new List<(double, double)>();
			var step = (limitedBounds.Item2 - limitedBounds.Item1) / _steps;

			var prevX = limitedBounds.Item1;
			var prevY = _function(limitedBounds.Item1);

			for (var x = limitedBounds.Item1; x < limitedBounds.Item2; x += step) {
				var y = _function(x);

				if (double.IsNaN(y))
					continue;

				if (y == 0)
					pairs.Add((x - step, x + step));

				if (DiffSign(prevY, y))
					pairs.Add((prevX, x));

				prevX = x;
				prevY = y;
			}

			return pairs;
		}

		private double Bisection((double, double) pair) {
			var (a, b) = pair;
			var c = 0.0;

			for (var i = 0ul; i < _steps; i++) {
				c = (a + b) / 2;
				var fc = _function(c);

				if (DiffSign(_function(a), fc))
					b = c;
				else if (DiffSign(_function(b), fc))
					a = c;
			}

			return c;
		}
	}
}
