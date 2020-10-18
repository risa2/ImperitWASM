using ImperitWASM.Shared.Motion;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.State
{
	public class Player
	{
		public int Id { get; }
		public string Name { get; }
		public Color Color { get; }
		public Password Password { get; }
		public int Money { get; }
		public bool Alive { get; }
		public ImmutableList<IPlayerAction> Actions { get; }
		public Player(int id, string name, Color color, Password password, int money, bool alive, ImmutableList<IPlayerAction> actions)
		{
			Id = id;
			Name = name;
			Color = color;
			Password = password;
			Money = money;
			Alive = alive;
			Actions = actions;
		}
		public virtual Player ChangeMoney(int amount) => new Player(Id, Name, Color, Password, Money + amount, Alive, Actions);
		public virtual Player Die() => new Player(Id, Name, Color, Password, 0, false, ImmutableList<IPlayerAction>.Empty);
		protected virtual Player WithActions(ImmutableList<IPlayerAction> new_actions) => new Player(Id, Name, Color, Password, Money, Alive, new_actions);
		public Player Add(params IPlayerAction[] actions) => WithActions(Actions.AddRange(actions));
		Player Action(PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(this, (player, action) => action.Perform(player, pap));
			return p.WithActions(a);
		}
		(Province, Player) Action(Province province, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(province, (province, action) => action.Perform(province, this, pap));
			return (p, WithActions(a));
		}
		public (ImmutableArray<Province>.Builder, Player) Act(PlayersAndProvinces pap)
		{
			var player = Action(pap);
			var new_provinces = ImmutableArray.CreateBuilder<Province>(pap.ProvincesCount);
			for (int i = 0; i < pap.ProvincesCount; ++i)
			{
				var (new_province, new_player) = player.Action(pap.Province(i), pap);
				player = new_player;
				new_provinces.Add(new_province);
			}
			return (new_provinces, player);
		}
		public override bool Equals(object? obj) => obj is Player p && p.Id == Id;
		public override int GetHashCode() => Id.GetHashCode();
		public static bool operator ==(Player? a, Player? b) => (a is null && b is null) || (a is Player x && x.Equals(b));
		public static bool operator !=(Player? a, Player? b) => ((a is null) != (b is null)) || (a is Player x && !x.Equals(b));
	}
}