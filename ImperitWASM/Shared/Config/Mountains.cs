using System.Collections.Immutable;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Mountains(string Name, Shape Shape, Soldiers Soldiers, ImmutableArray<SoldierType> ExtraTypes)
		: Region(Name, Shape, Soldiers, ExtraTypes)
	{
		public override Color Stroke(Settings settings) => settings.MountainsColor;
		public override int StrokeWidth(Settings settings) => settings.MountainsWidth;

		public override bool Sailable => true;
		public override ImmutableArray<string> Text(Soldiers present) => ImmutableArray.Create(Name, present.ToString());

		public virtual bool Equals(Sea? region) => Name == region?.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
