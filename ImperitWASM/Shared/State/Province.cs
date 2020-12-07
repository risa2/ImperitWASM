using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public abstract record Province(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings)
	{
		public Description Description => new Description(Name, Text);
		public abstract ImmutableArray<string> Text { get; }
		public ImmutableArray<Point> Border => Shape.Border;
		public Point Center => Shape.Center;
		public Province GiveUpTo(Player player, Soldiers soldiers) => this with { Player = player, Soldiers = soldiers };
		public Province GiveUpTo(Player p) => GiveUpTo(p, new Soldiers());
		public Province Revolt() => GiveUpTo(Settings.Savage, DefaultSoldiers);

		Province WithActions(ImmutableList<IProvinceAction> new_actions) => this with { Actions = new_actions };
		public Province Add(params IProvinceAction[] actions) => WithActions(Actions.AddRange(actions));
		public Province Replace(Func<IProvinceAction, IProvinceAction> replacer) => WithActions(Actions.Select(replacer).ToImmutableList());

		Province ActOnYourself(PlayersAndProvinces pap)
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
		public Soldiers NextSoldiers(PlayersAndProvinces pap) => ActOnYourself(pap).Soldiers;
		public Soldiers MaxAttackers(PlayersAndProvinces pap, Province to) => Soldiers.MaxAttackers(pap, this, to);
		public int AttackPower => Soldiers.AttackPower;
		public int DefensePower => Soldiers.DefensePower;
		public int Power => Soldiers.Power;
		public int DefaultDefensePower => DefaultSoldiers.DefensePower;

		public Province Subtract(Soldiers army) => GiveUpTo(Player, Soldiers.Subtract(army));
		public Province AttackedBy(Player p, Soldiers s) => GiveUpTo(s.AttackPower > Soldiers.DefensePower ? p : Player, Soldiers.AttackedBy(s));
		public Province ReinforcedBy(Soldiers another) => GiveUpTo(Player, Soldiers.Add(another));
		public bool Occupied => Player is not Savage;
		public bool IsAllyOf(Player p) => p == Player;
		public bool IsAllyOf(Province prov) => prov.Player == Player;
		public bool IsEnemyOf(Player p) => Occupied && p != Player;

		public bool Has(Soldiers soldiers) => Soldiers.Contains(soldiers);
		public bool HasSoldiers => Soldiers.Any;
		public bool CanSoldiersSurvive => Soldiers.CanSurviveIn(this);
		public bool CanSurviveWithout(Soldiers s) => Subtract(s).CanSoldiersSurvive;
		public bool CanAnyMove(PlayersAndProvinces pap, Province to) => Soldiers.Any(reg => reg.CanMoveAlone(pap, this, to));
		public bool CanMove(PlayersAndProvinces pap, Province to, Player player, Soldiers soldiers) => IsAllyOf(player) && Soldiers.CanMove(pap, this, to) && CanSurviveWithout(soldiers);

		public virtual Color Fill => new Color();
		public virtual Color Stroke => new Color();
		public virtual int StrokeWidth => 0;

		public virtual bool Equals(Province? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}