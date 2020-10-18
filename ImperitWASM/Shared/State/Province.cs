using ImperitWASM.Shared.Motion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public abstract class Province : IEnumerable<Point>
	{
		public readonly int Id;
		public readonly string Name;
		public readonly Shape Shape;
		public readonly Army Army, DefaultArmy;
		public readonly int Earnings;
		public readonly ImmutableList<IProvinceAction> Actions;
		protected readonly Settings settings;
		public Province(int id, string name, Shape shape, Army army, Army defaultArmy, int earnings, ImmutableList<IProvinceAction> actions, Settings set)
		{
			Id = id;
			Name = name;
			Shape = shape;
			Army = army;
			DefaultArmy = defaultArmy;
			Earnings = earnings;
			Actions = actions;
			settings = set;
		}
		public abstract Province GiveUpTo(Army army);
		protected abstract Province WithActions(ImmutableList<IProvinceAction> new_actions);
		public Province Add(params IProvinceAction[] actions) => WithActions(Actions.AddRange(actions));
		public Province Replace(Func<IProvinceAction, IProvinceAction> replacer) => WithActions(Actions.Select(replacer).ToImmutableList());
		public Province GiveUpTo(Player p) => GiveUpTo(new Army(new Soldiers(), p));
		Province Act(Player active, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(this, (province, action) => action.Perform(province, active, pap));
			return p.WithActions(a);
		}
		(Province, Player) Act(Player player, Player active, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(player, (player, action) => action.Perform(player, active, pap));
			return (WithActions(a), p);
		}
		public (Province, ImmutableArray<Player>.Builder) Act(PlayersAndProvinces pap, Player active)
		{
			var province = Act(active, pap);
			var new_players = ImmutableArray.CreateBuilder<Player>(pap.PlayersCount);
			for (int i = 0; i < pap.PlayersCount; ++i)
			{
				var (new_province, new_player) = province.Act(pap.Player(i), active, pap);
				province = new_province;
				new_players.Add(new_player);
			}
			return (province, new_players);
		}
		public Province Revolt() => GiveUpTo(DefaultArmy);
		public Province Subtract(Soldiers army) => GiveUpTo(Army.Subtract(army));
		public Province AttackedBy(Army another) => GiveUpTo(Army.AttackedBy(another));
		public Province ReinforcedBy(Soldiers another) => GiveUpTo(Army.Join(another));
		public bool IsAllyOf(Player p) => Army.IsAllyOf(p);
		public bool IsAllyOf(Army army) => Army.IsAllyOf(army);
		public bool IsAllyOf(Province prov) => IsAllyOf(prov.Army);
		public Soldiers Soldiers => Army.Soldiers;
		public Soldiers DefaultSoldiers => DefaultArmy.Soldiers;
		public IEnumerable<SoldierType> SoldierTypes => Soldiers.Types;
		public bool Occupied => !Army.IsControlledBySavage;
		public bool CanSoldiersSurvive => Soldiers.CanSurviveIn(this);
		public virtual Color Fill => new Color();
		public virtual Color Stroke => new Color();
		public virtual int StrokeWidth => 0;
		public virtual string[] Text => System.Array.Empty<string>();
		public IEnumerator<Point> GetEnumerator() => Shape.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public Point Center => Shape.Center;

		public override bool Equals(object? obj) => obj != null && obj is Province p && p.Id == Id;
		public override int GetHashCode() => Id.GetHashCode();
		public static bool operator ==(Province? a, Province? b) => (a is null && b is null) || (a is object x && x.Equals(b));
		public static bool operator !=(Province? a, Province? b) => ((a is null) != (b is null)) || (a is object x && !x.Equals(b));
	}
}