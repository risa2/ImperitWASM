using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public sealed record Player(PlayerIdentity Id, int Money, bool Alive, ImmutableList<IAction> Actions, Settings Settings, Password Password, bool Active)
	{
		public string Name => Id.Name;
		public Color Color => Id.Color;
		public int GameId => Id.GameId;
		public int Order => Id.Order;
		public bool Human => Id.Human;

		public Description Description => new Description(Name, ImmutableArray<string>.Empty);
		public int MaxBorrowable => Settings.Discount(Settings.DebtLimit - Debt);
		public int MaxUsableMoney => Money + MaxBorrowable;
		public Player ChangeMoney(int amount) => this with { Money = amount + Money };
		public Player Earn(Provinces provinces) => ChangeMoney(provinces.IncomeOf(Id));
		
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
		public Power Power(Provinces provinces)
		{
			var my = provinces.ControlledBy(Id).ToImmutableArray();
			return new Power(Alive, my.Sum(p => p.Score), my.Sum(p => p.Earnings), Money - Debt, my.Sum(p => p.Power));
		}

		public bool Equals(Player? p) => p is not null && Id == p.Id;
		public override int GetHashCode() => Id.GetHashCode();

		public (Player, Provinces) Think(IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			return new Brain(this, settings).Think(players, provinces, game);
		}
		public static Color ColorOf(Player? p) => p?.Color ?? new Color();
		public override string ToString() => Name;
	}
}