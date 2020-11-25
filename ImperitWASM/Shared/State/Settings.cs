using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.SettingsConverter))]
	public record Settings(Ratio DefaultInstability, double Interest, int DebtLimit, int DefaultMoney, int MountainsWidth, Color LandColor, Color MountainsColor, Color SeaColor, ImmutableArray<SoldierType> SoldierTypes, ImmutableDictionary<SoldierType, int> SoldierTypeIndices, int FinalLandsCount, TimeSpan CountdownTime, Graph Graph, ImmutableArray<ProvinceData> ProvinceData)
	{
		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		public Provinces Provinces => new Provinces(ProvinceData.Select(p => p.Build(this, new Savage())).ToImmutableArray(), Graph);
	}
}