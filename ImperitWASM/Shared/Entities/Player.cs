using System;
using System.Collections.Immutable;
using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public abstract class Player : IEquatable<Player>
	{
		private int _id;
		public Color Color { get; }
		public string Name { get; }
		public int Money { get; }
		public bool Alive { get; }
		public ImmutableList<PlayerAction> Actions { get; }
		public Player(Color color, string name, int money, bool alive, ImmutableList<PlayerAction> actions)
		{
			Color = color;
			Name = name;
			Money = money;
			Alive = alive;
			Actions = actions;
		}
		public Description Description => new Description(Name);
		public abstract Player ChangeMoney(int amount);
		public abstract Player Die();
		protected abstract Player WithActions(ImmutableList<PlayerAction> new_actions);
		public Player Add(params PlayerAction[] actions) => WithActions(Actions.AddRange(actions));
		public Player Replace<T>(Predicate<T> cond, T value, Func<T, T, T> interact) where T : PlayerAction
		{
			return WithActions(Actions.Replace(cond, interact, value));
		}
		Player Action(Settings set, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(this, (player, action) => action.Perform(set, player, pap));
			return p.WithActions(a);
		}
		(Province, Player) Action(Settings set, Province province, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(province, (province, action) => action.Perform(set, province, this, pap));
			return (p, WithActions(a));
		}
		public (ImmutableArray<Province>.Builder, Player) Act(Settings set, PlayersAndProvinces pap)
		{
			var player = Action(set, pap);
			var new_provinces = ImmutableArray.CreateBuilder<Province>(pap.ProvincesCount);
			foreach (var province in pap.Provinces)
			{
				var (new_province, new_player) = player.Action(set, province, pap);
				player = new_player;
				new_provinces.Add(new_province);
			}
			return (new_provinces, player);
		}
		public bool IsLivingHuman => this is Human && Alive;
		public bool Equals(Player? obj) => obj?.Name == Name;
		public override bool Equals(object? obj) => Equals(obj as Player);
		public override int GetHashCode() => Description.GetHashCode();
		public static bool operator ==(Player? a, Player? b) => a?.Name == b?.Name;
		public static bool operator !=(Player? a, Player? b) => a?.Name != b?.Name;
	}
}