using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.State
{
	public class Shape : IEnumerable<Point>
	{
		readonly ImmutableArray<Point> border;
		public readonly Point Center;
		public Shape(ImmutableArray<Point> points, Point center)
		{
			border = points;
			Center = center;
		}
		public IEnumerator<Point> GetEnumerator() => (border as IEnumerable<Point>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}