using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Actions;

namespace ImperitWASM.Shared.State
{
	public abstract record Player(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions, Settings Settings)
	{
		protected static readonly ImmutableList<IPlayerAction> DefaultActions = ImmutableList.Create<IPlayerAction>(new EndTurn());
		public Description Description => new Description(Name, ImmutableArray<string>.Empty);
		public Player ChangeMoney(int amount) => this with { Money = amount + Money };
		protected Player WithActions(ImmutableList<IPlayerAction> new_actions) => this with { Actions = new_actions };
		public Player Die() => this with { Money = 0, Alive = false, Actions = ImmutableList<IPlayerAction>.Empty };
		public Player Add(params IPlayerAction[] actions) => WithActions(Actions.AddRange(actions));
		public Player Replace<T>(Predicate<T> cond, T value, Func<T, T, T> interact) where T : IPlayerAction
		{
			return WithActions(Actions.Replace(cond, interact, value));
		}
		public Player Borrow(int amount)
		{
			return ChangeMoney(amount).Replace(a => true, new Loan(amount, Settings), (x, y) => new Loan(x.Debt + y.Debt, Settings));
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
		public int Debt => Actions.OfType<Loan>().Sum(a => a.Debt);
		public PlayerPower Power(ImmutableArray<Province> provinces) => new PlayerPower(Alive, provinces.OfType<Land>().Sum(p => p.Earnings), provinces.Count(p => p is Land), Money - Debt, provinces.Sum(p => p.Power), provinces.Count(p => p is Land { IsFinal: true }));
		public virtual bool Equals(Player? obj) => obj is not null && Name == obj.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}