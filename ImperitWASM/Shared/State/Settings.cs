using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public class Settings
	{
		public readonly Probability DefaultInstability;
		public readonly double Interest;
		public readonly int DebtLimit, DefaultMoney;
		public readonly int MountainsWidth;
		public readonly Color LandColor, MountainsColor, SeaColor;
		public readonly ImmutableArray<string> RobotNames;
		public readonly ImmutableArray<SoldierType> SoldierTypes;
		public Settings(int debtLimit, Probability defaultInstability, int defaultMoney, double interest, Color landColor, Color mountainsColor, int mountainsWidth, ImmutableArray<string> robotNames, Color seaColor, ImmutableArray<SoldierType> soldierTypes)
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
		public Probability Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
	}
}