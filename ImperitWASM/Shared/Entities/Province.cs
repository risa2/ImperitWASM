using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public abstract class Province : IEnumerable<Point>, IEquatable<Province>
	{
		private int _id;
		public string Name { get; }
		public Player Player { get; }
		public Soldiers Soldiers { get; }
		public Soldiers DefaultSoldiers { get; }
		public Shape Shape { get; }
		public ImmutableList<ProvinceAction> Actions { get; }
		public Province(string name, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<ProvinceAction> actions)
		{
			Name = name;
			Shape = shape;
			Player = player;
			Soldiers = soldiers;
			DefaultSoldiers = defaultSoldiers;
			Actions = actions;
		}
		public abstract Description Description { get; }
		public string Text => Description.Text;
		public abstract Province GiveUpTo(Player player, Soldiers soldiers);
		public Province GiveUpTo(Player p) => GiveUpTo(p, Soldiers.Empty);
		public Province Revolt() => GiveUpTo(new Savage(), DefaultSoldiers);
		protected abstract Province WithActions(ImmutableList<ProvinceAction> new_actions);
		public Province Add(params ProvinceAction[] actions) => WithActions(Actions.AddRange(actions));
		public Province Replace(Func<ProvinceAction, ProvinceAction> replacer) => WithActions(Actions.Select(replacer).ToImmutableList());
		public Province ActOnYourself(Settings settings, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(this, (province, action) => action.Perform(settings, province, pap));
			return p.WithActions(a);
		}
		(Province, Player) Act(Settings settings, Player player, PlayersAndProvinces pap)
		{
			var (a, p) = Actions.Fold(player, (player, action) => action.Perform(settings, player, pap));
			return (WithActions(a), p);
		}
		public (Province, ImmutableArray<Player>.Builder) Act(Settings settings, PlayersAndProvinces pap)
		{
			var province = ActOnYourself(settings, pap);
			var new_players = ImmutableArray.CreateBuilder<Player>(pap.PlayersCount);
			foreach (var player in pap.Players)
			{
				var (new_province, new_player) = province.Act(settings, player, pap);
				province = new_province;
				new_players.Add(new_player);
			}
			return (province, new_players);
		}
		public Province Subtract(Soldiers soldiers) => GiveUpTo(Player, Soldiers.Subtract(soldiers));
		public Province AttackedBy(Player p, Soldiers s) => Soldiers.AttackedBy(s, s => GiveUpTo(p, s), s => GiveUpTo(Player, s));
		public Province ReinforcedBy(Soldiers another) => GiveUpTo(Player, Soldiers.Add(another));
		public bool IsAllyOf(Player p) => p == Player;
		public bool IsAllyOf(Province prov) => prov.Player == Player;
		public bool Occupied => !(Player is Savage);
		public bool CanSoldiersSurvive => Soldiers.CanSurviveIn(this);
		public virtual Color Fill(Settings s) => new Color();
		public virtual Color Stroke(Settings s) => new Color();
		public virtual int StrokeWidth(Settings s) => 0;
		public IEnumerator<Point> GetEnumerator() => Shape.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public Point Center => Shape.Center;

		public bool Equals(Province? other) => other?.Name == Name;
		public override bool Equals(object? obj) => Equals(obj as Province);
		public override int GetHashCode() => Name.GetHashCode();
		public static bool operator ==(Province? a, Province? b) => a?.Name == b?.Name;
		public static bool operator !=(Province? a, Province? b) => a?.Name != b?.Name;
	}
}