using System.Collections.Immutable;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Sea(int Id, string Name, Shape Shape, Soldiers Soldiers, ImmutableArray<SoldierType> ExtraTypes)
		: Region(Id, Name, Shape, Soldiers, ExtraTypes)
	{
		public override Color Fill(Settings settings, Color player_color) => player_color.Mix(settings.SeaColor);
		public override bool Sailable => true;
		public override ImmutableArray<string> Text(Soldiers present) => ImmutableArray.Create(Name, present.ToString());

		public virtual bool Equals(Sea? region) => Name == region?.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
