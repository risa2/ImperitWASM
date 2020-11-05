using System;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Server
{
	public class MoveInfo
	{
		public bool Possible { get; set; }
		public bool CanAttack { get; set; }
		public bool CanReinforce { get; set; }
		public string FromName { get; set; } = "";
		public string ToName { get; set; } = "";
		public string FromSoldiers { get; set; } = "";
		public string ToSoldiers { get; set; } = "";
		public Description[] Soldiers { get; set; } = Array.Empty<Description>();
		public MoveInfo() { }
		public MoveInfo(bool possible, bool canAttack, bool canReinforce, string fromName, string toName, string fromSoldiers, string toSoldiers, Description[] soldiers)
		{
			Possible = possible;
			CanAttack = canAttack;
			CanReinforce = canReinforce;
			FromName = fromName;
			ToName = toName;
			FromSoldiers = fromSoldiers;
			ToSoldiers = toSoldiers;
			Soldiers = soldiers;
		}
	}
}
