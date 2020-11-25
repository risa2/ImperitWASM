using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public abstract record Player(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions)
	{
		public Description Description => new Description(Name);
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
		public virtual bool Equals(Player? obj) => obj is not null && obj.Name == obj.Name;
		public override int GetHashCode() => Description.GetHashCode();
		public PlayerPower Power(ImmutableArray<Province> provinces) => new PlayerPower(Alive, provinces.OfType<Land>().Sum(p => p.Earnings), provinces.Count(p => p is Land), Money, provinces.Sum(p => p.Soldiers.Power), provinces.Count(p => p is Land l && l.IsFinal));
	}
}