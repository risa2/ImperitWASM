using System;
using System.Collections.Immutable;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Land(int Id, string Name, Shape Shape, Soldiers Soldiers, ImmutableArray<SoldierType> ExtraTypes, int Earnings, Ratio DefaultInstability, bool IsStart, bool IsFinal, bool HasPort)
		: Region(Id, Name, Shape, Soldiers, ExtraTypes)
	{
		public override Color Fill(Settings settings) => settings.LandColor;

		public override bool Inhabitable => IsStart;
		public override bool Sailable => HasPort;
		public override bool Port => HasPort;
		public override bool Mainland => true;
		public override bool Dry => true;

		public override int Price(Soldiers now) => now.Price + now.DefensePower + Earnings * 2;
		public override int Score => IsFinal ? 1 : 0;
		public override int Income => Earnings;

		public override Ratio Instability(Soldiers present) => DefaultInstability.Adjust(Math.Max(DefensePower - present.DefensePower, 0), DefensePower);

		string Suffix => (IsFinal ? "\u2605" : "") + (HasPort ? "\u2693" : "");
		public override ImmutableArray<string> Text(Soldiers present) => ImmutableArray.Create(Name + Suffix, present.ToString(), Earnings + "\uD83D\uDCB0");

		public virtual bool Equals(Land? region) => Name == region?.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
