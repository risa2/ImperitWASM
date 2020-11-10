using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public class Move : ICommand
	{
		public readonly Player Player;
		public readonly Province From, To;
		public readonly Soldiers Soldiers;
		public Move(Player player, Province from, Province to, Soldiers soldiers)
		{
			Player = player;
			From = from;
			To = to;
			Soldiers = soldiers;
		}
		public virtual bool Allowed(Settings settings, PlayersAndProvinces pap)
		{
			return From.IsAllyOf(Player) && Soldiers.CanMove(pap, From, To) && To.Subtract(Soldiers).CanSoldiersSurvive;
		}
		public Province Perform(Settings settings, Province province)
		{
			return province == From ? province.Subtract(Soldiers) : province == To ? province.Add(new Manoeuvre(Player, Soldiers)) : province;
		}
	}
}