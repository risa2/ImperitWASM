using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Server.Load
{
	public class JsonSettings : IEntity<Settings, bool>
	{
		public int DebtLimit { get; set; }
		public int DefaultInstability { get; set; }
		public int DefaultMoney { get; set; }
		public double Interest { get; set; }
		public string LandColor { get; set; } = "";
		public string MountainsColor { get; set; } = "";
		public int MountainsWidth { get; set; }
		public ImmutableArray<string> RobotNames { get; set; }
		public string SeaColor { get; set; } = "";
		public IEnumerable<JsonSoldierType>? SoldierTypes { get; set; }
		public int FinalLandsCount { get; set; }
		public Settings Convert(int _, bool __) => new Settings(DebtLimit, new Probability(DefaultInstability), DefaultMoney, Interest, Color.Parse(LandColor), Color.Parse(MountainsColor), MountainsWidth, RobotNames, Color.Parse(SeaColor), SoldierTypes.Select((t, i) => t.Convert(i, false)).ToImmutableArray(), FinalLandsCount);
	}
}