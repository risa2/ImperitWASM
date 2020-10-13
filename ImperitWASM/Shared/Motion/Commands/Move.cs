using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public abstract class Move : ICommand
	{
		public readonly Player Player;
		public readonly Province From, To;
		public readonly Army Army;
		public Move(Player player, Province from, Province to, Army army)
		{
			Player = player;
			From = from;
			To = to;
			Army = army;
		}
		public virtual bool Allowed(PlayersAndProvinces pap)
		{
			return From.IsAllyOf(Player) && Army.CanMove(pap, From.Id, To.Id) && To.Subtract(Army.Soldiers).CanSoldiersSurvive;
		}
		protected abstract IAction Action { get; }
		public Province Perform(Province province)
		{
			return province == From ? province.Subtract(Army.Soldiers) : province == To ? province.Add(Action) : province;
		}
		public Soldiers Soldiers => Army.Soldiers;
	}
}