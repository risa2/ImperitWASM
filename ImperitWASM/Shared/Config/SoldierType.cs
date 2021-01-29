using System.Collections.Immutable;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	[Newtonsoft.Json.JsonConverter(typeof(Conversion.NewtonsoftSoldierType))]
	public abstract record SoldierType(Description Description, int AttackPower, int DefensePower, int Weight, int Price)
	{
		public string Name => Description.Name;
		public string Symbol => Description.Symbol;
		public ImmutableArray<string> Text => Description.Text;
		public int Power => AttackPower + DefensePower;
		public abstract bool IsRecruitable(Region province);
		public abstract int CanSustain(Region province);
		public abstract int CanMove(Provinces provinces, Province from, Province to);
		public bool CanMoveAlone(Provinces provinces, Province from, Province to) => CanMove(provinces, from, to) >= Weight;
	}
}
