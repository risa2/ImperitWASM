using ImperitWASM.Shared.State;
using ImperitWASM.Shared.Conversion;
using System.Collections.Immutable;

namespace ImperitWASM.Server.Load
{
	public class JsonShape : IEntity<Shape, bool>
	{
		public ImmutableArray<Point> Border { get; set; }
		public Point Center { get; set; }
		public Shape Convert(int _i, bool _b) => new Shape(Border, Center);
	}
}