using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Mountains(string Name, Shape Shape, Settings Settings)
		: Province(Name, Shape, new Savage(Settings), new Soldiers(), new Soldiers(), ImmutableList<IProvinceAction>.Empty, Settings)
	{
		public override ImmutableArray<string> Text => ImmutableArray<string>.Empty;
		public virtual bool Equals(Mountains? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override Color Stroke => Settings.MountainsColor;
		public override int StrokeWidth => Settings.MountainsWidth;
	}
}