using System.Collections.Immutable;

namespace ImperitWASM.Shared.State
{
	public record Shape(ImmutableArray<Point> Border, Point Center);
}