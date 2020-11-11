using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.SettingsConverter))]
	public class Settings
	{
		public readonly Ratio DefaultInstability;
		public readonly double Interest;
		public readonly int DebtLimit, DefaultMoney;
		public readonly int MountainsWidth;
		public readonly Color LandColor, MountainsColor, SeaColor;
		public readonly ImmutableArray<SoldierType> SoldierTypes;
		public readonly ImmutableDictionary<SoldierType, int> SoldierTypeIndices;
		public readonly int FinalLandsCount;
		public readonly TimeSpan CountdownTime;
		public readonly Graph Graph;
		public readonly ImmutableArray<ProvinceData> ProvinceData;
		public Settings(int debtLimit, Ratio defaultInstability, int defaultMoney, double interest, Color landColor, Color mountainsColor, int mountainsWidth, Color seaColor, ImmutableArray<SoldierType> soldierTypes, int finalLandsCount, TimeSpan countdownTime, Graph graph, ImmutableArray<ProvinceData> provinceData)
		{
			DebtLimit = debtLimit;
			DefaultInstability = defaultInstability;
			DefaultMoney = defaultMoney;
			Interest = interest;
			LandColor = landColor;
			MountainsColor = mountainsColor;
			MountainsWidth = mountainsWidth;
			SeaColor = seaColor;
			SoldierTypes = soldierTypes;
			SoldierTypeIndices = SoldierTypes.Lookup();
			FinalLandsCount = finalLandsCount;
			CountdownTime = countdownTime;
			Graph = graph;
			ProvinceData = provinceData;
		}
		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		public Provinces Provinces => new Provinces(ProvinceData.Select(p => p.Build(this, new Savage())).ToImmutableArray(), Graph);
	}
}