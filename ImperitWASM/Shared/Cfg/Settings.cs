using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cfg
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
		public readonly ImmutableArray<ProvinceFactory> Provinces;
		public Settings(int debtLimit, Ratio defaultInstability, int defaultMoney, double interest, Color landColor, Color mountainsColor, int mountainsWidth, Color seaColor, ImmutableArray<SoldierType> soldierTypes, int finalLandsCount, TimeSpan countdownTime, Graph graph, ImmutableArray<ProvinceFactory> provinces)
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
			Provinces = provinces;
		}
		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		public bool CountdownElapsed(DateTime start) => DateTime.UtcNow - start >= CountdownTime;
	}
}