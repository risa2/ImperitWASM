using System;
using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public abstract class Player : IEquatable<Player>
	{
		public Color Color { get; }
		public Description Description { get; }
		public int Money { get; }
		public bool Alive { get; }
		public ImmutableList<IPlayerAction> Actions { get; }
		public Player(Color color, Description description, int money, bool alive, ImmutableList<IPlayerAction> actions)
		{
			Color = color;
			Description = description;
			Money = money;
			Alive = alive;
			Actions = actions;
		}
		public string Name => Description.Name;
		public abstract Player ChangeMoney(int amount);
		public abstract Player Die();
		protected abstract Player WithActions(ImmutableList<IPlayerAction> new_actions);
		public Player Add(params IPlayerAction[] actions) => WithActions(Actions.AddRange(actions));
		public Player Replace<T>(Predicate<T> cond, T value, Func<T, T, T> interact) where T : IPlayerAction
		{
			return WithActions(Actions.Replace(cond, interact, value));
		}

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
			foreach (var province in pap.Provinces)
			{
				var (new_province, new_player) = player.Action(province, pap);
				player = new_player;
				new_provinces.Add(new_province);
			}
			return (new_provinces, player);
		}
		public bool IsLivingHuman => this is Human && Alive;
		public bool Equals(Player? obj) => obj != null && obj.Description == obj.Description;
		public override bool Equals(object? obj) => Equals(obj as Player);
		public override int GetHashCode() => Description.GetHashCode();
		public static bool operator ==(Player? a, Player? b) => a?.Description == b?.Description;
		public static bool operator !=(Player? a, Player? b) => a?.Description != b?.Description;
	}
}