using System.Collections.Immutable;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Mountains(int Id, string Name, Shape Shape)
		: Region(Id, Name, Shape, new Soldiers(), ImmutableArray<SoldierType>.Empty)
	{
		public override Color Stroke(Settings settings) => settings.MountainsColor;
		public override int StrokeWidth(Settings settings) => settings.MountainsWidth;

		public override bool Sailable => true;
		public override ImmutableArray<string> Text(Soldiers present) => ImmutableArray.Create(Name, present.ToString());

		public virtual bool Equals(Sea? region) => Name == region?.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
