using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Mountains(string Name, Shape Shape, Settings Settings)
		: Province(new Description(Name, ImmutableArray<string>.Empty), Shape, new Savage(), new Soldiers(), new Soldiers(), ImmutableList<IProvinceAction>.Empty)
	{
		public virtual bool Equals(Mountains? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override Color Stroke => Settings.MountainsColor;
		public override int StrokeWidth => Settings.MountainsWidth;
	}
}