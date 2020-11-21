using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.ShapeConverter))]
	public class Shape : IEnumerable<Point>
	{
		private readonly ImmutableArray<Point> border;
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