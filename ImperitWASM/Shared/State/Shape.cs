using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.ShapeConverter))]
	public record Shape(ImmutableArray<Point> Border, Point Center);
}