using System.Collections.Immutable;

namespace ImperitWASM.Shared.Config
{
	public record Shape(ImmutableArray<Point> Border, Point Center);
}