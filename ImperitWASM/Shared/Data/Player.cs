using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public sealed record Player(Color Color, string Name, int Money, bool Alive, ImmutableList<IAction> Actions, Settings Settings, bool Human, Password Password, bool Active)
	{
		public Description Description => new Description(Name, ImmutableArray<string>.Empty);
		public int MaxBorrowable => Settings.Discount(Settings.DebtLimit - Debt);
		public int MaxUsableMoney => Money + MaxBorrowable;
		public Player ChangeMoney(int amount) => this with { Money = amount + Money };
		public Player Earn(Provinces provinces) => ChangeMoney(provinces.IncomeOf(this));
		
		public Player Die() => this with { Money = 0, Alive = false, Actions = ImmutableList<IAction>.Empty };
		public Player Add(params IAction[] actions) => this with { Actions = Actions.AddRange(actions) };
		public Player Replace<T>(Predicate<T> cond, T value, Func<T, T, T> interact) where T : IAction
		{
			return this with { Actions = Actions.Replace(cond, interact, value) };
		}

		public Player Borrow(int amount) => ChangeMoney(amount).Replace(a => true, new Loan(amount), (x, y) => x + y);
		public Player Pay(int amount) => (amount > Money ? Borrow(amount - Money) : this).ChangeMoney(-amount);

		public (Player, Provinces) Act(Provinces provinces, Settings settings)
		{
			var (new_player, new_provinces) = Actions.Aggregate((this with { Actions = ImmutableList<IAction>.Empty }, provinces), (pair, action) =>
			{
				var (player, new_provinces, new_action) = action.Perform(pair.Item1, pair.provinces, settings);
				return (new_action is not null ? player.Add(new_action) : player, new_provinces);
			});
			return (new_player, new_provinces);
		}

		public Player InvertActive => this with { Active = !Active };
		public bool LivingHuman => Human && Alive;
		public int Debt => Actions.OfType<Loan>().Sum(a => a.Debt);
		public PlayerPower Power(ImmutableArray<Province> provinces) => new PlayerPower(Alive, provinces.Sum(p => p.Score), provinces.Sum(p => p.Earnings), provinces.Count(p => p.Mainland), Money - Debt, provinces.Sum(p => p.Power));

		public bool Equals(Player? obj) => obj is not null && Name == obj.Name;
		public override int GetHashCode() => Name.GetHashCode();

		public (Player, Provinces) Think(IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return new Brain(this, settings).Think(players, provinces);
		}
		public static Color ColorOf(Player? p) => p?.Color ?? new Color();
	}
}