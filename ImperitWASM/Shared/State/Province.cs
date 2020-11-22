using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public abstract class Province : IEnumerable<Point>, IEquatable<Province>
	{
		public readonly Description Description;
		public readonly Shape Shape;
		public readonly Player Player;
		public readonly Soldiers Soldiers, DefaultSoldiers;
		public readonly ImmutableList<IProvinceAction> Actions;
		public Province(Description description, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<IProvinceAction> actions)
		{
			Description = description;
			Shape = shape;
			Player = player;
			Soldiers = soldiers;
			DefaultSoldiers = defaultSoldiers;
			Actions = actions;
		}
		public string Name => Description.Name;
		public string Text => Description.Text;
		public abstract Province GiveUpTo(Player player, Soldiers soldiers);
		public Province GiveUpTo(Player p) => GiveUpTo(p, new Soldiers());
		public Province Revolt() => GiveUpTo(new Savage(), DefaultSoldiers);
		protected abstract Province WithActions(ImmutableList<IProvinceAction> new_actions);
		public Province Add(params IProvinceAction[] actions) => WithActions(Actions.AddRange(actions));
		public Province Replace(Func<IProvinceAction, IProvinceAction> replacer) => WithActions(Actions.Select(replacer).ToImmutableList());
		public Province ActOnYourself(PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(this, (province, action) => action.Perform(province, pap));
			return p.WithActions(a);
		}

		(Province, Player) Act(Player player, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(player, (player, action) => action.Perform(player, pap));
			return (WithActions(a), p);
		}
		public (Province, ImmutableArray<Player>.Builder) Act(PlayersAndProvinces pap)
		{
			var province = ActOnYourself(pap);
			var new_players = ImmutableArray.CreateBuilder<Player>(pap.PlayersCount);
			foreach (var player in pap.Players)
			{
				var (new_province, new_player) = province.Act(player, pap);
				province = new_province;
				new_players.Add(new_player);
			}
			return (province, new_players);
		}
		public Province Subtract(Soldiers army) => GiveUpTo(Player, Soldiers.Subtract(army));
		public Province AttackedBy(Player p, Soldiers s) => s.AttackPower > Soldiers.DefensePower ? GiveUpTo(p, Soldiers.AttackedBy(s)) : GiveUpTo(Player, Soldiers.AttackedBy(s));
		public Province ReinforcedBy(Soldiers another) => GiveUpTo(Player, Soldiers.Add(another));
		public bool IsAllyOf(Player p) => p == Player;
		public bool IsAllyOf(Province prov) => prov.Player == Player;
		public IEnumerable<SoldierType> SoldierTypes => Soldiers.Types;
		public bool Occupied => !(Player is Savage);
		public bool CanSoldiersSurvive => Soldiers.CanSurviveIn(this);
		public virtual Color Fill => new Color();
		public virtual Color Stroke => new Color();
		public virtual int StrokeWidth => 0;
		public IEnumerator<Point> GetEnumerator() => Shape.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public Point Center => Shape.Center;

		public bool Equals(Province? other) => other?.Description == Description;
		public override bool Equals(object? obj) => Equals(obj as Province);
		public override int GetHashCode() => Description.GetHashCode();
		public static bool operator ==(Province? a, Province? b) => a?.Description == b?.Description;
		public static bool operator !=(Province? a, Province? b) => a?.Description != b?.Description;
	}
}