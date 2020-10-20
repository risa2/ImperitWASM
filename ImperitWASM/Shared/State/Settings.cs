using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public class Settings
	{
		public readonly double DefaultInstability, Interest;
		public readonly int DebtLimit, DefaultMoney;
		public readonly int MountainsWidth;
		public readonly Color LandColor, MountainsColor, SeaColor;
		public readonly ImmutableArray<string> RobotNames;
		public readonly ImmutableArray<SoldierType> SoldierTypes;
		public Settings(int debtLimit, double defaultInstability, int defaultMoney, double interest, Color landColor, Color mountainsColor, int mountainsWidth, ImmutableArray<string> robotNames, Color seaColor, ImmutableArray<SoldierType> soldierTypes)
		{
			DebtLimit = debtLimit;
			DefaultInstability = defaultInstability;
			DefaultMoney = defaultMoney;
			Interest = interest;
			LandColor = landColor;
			MountainsColor = mountainsColor;
			MountainsWidth = mountainsWidth;
			RobotNames = robotNames;
			SeaColor = seaColor;
			SoldierTypes = soldierTypes;
		}
		public double Instability(Soldiers now, Soldiers start) => DefaultInstability * Math.Max(start.DefensePower - now.DefensePower - 1, -1) / start.DefensePower;
		public IEnumerable<SoldierType> RecruitableTypes(Province where, Player player) => SoldierTypes.Where(t => t.IsRecruitable(where) && t.Price <= player.Money);
	}
}