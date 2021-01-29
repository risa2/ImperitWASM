using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public sealed record Province(Region Region, Player? Player, Soldiers Soldiers, Settings Settings)
	{
		public int Order => Region.Order;
		public string Name => Region.Name;
		public ImmutableArray<string> Text => Region.Text(Soldiers);
		public ImmutableArray<Point> Border => Region.Border;
		public Point Center => Region.Center;
		public Soldiers DefaultSoldiers => Region.Soldiers;

		public Province RuledBy(Player player) => this with { Player = player };
		public Province WithSoldiers(Soldiers soldiers) => this with { Soldiers = soldiers };
		public Province Revolt() => this with { Player = null, Soldiers = DefaultSoldiers };

		public bool Recruitable(SoldierType type) => Region.Recruitable(type);
		public bool Unstable => !CanPersist || !HasSoldiers || Region.Unstable(Settings, Soldiers);
		public bool Shaky(Player active) => !CanPersist || !Soldiers.Any || (IsAllyOf(active) && Region.Shaky(Settings, Soldiers));
		public Province RevoltIfShaky(Player active) => Shaky(active) ? Revolt() : this;

		public bool Inhabited => Player is not null;
		public bool Inhabitable => !Inhabited && Region.Inhabitable;
		public bool Sailable => Region.Sailable;
		public bool Mainland => Region.Mainland;
		public bool Dry => Region.Dry;
		public bool Port => Region.Port;

		public int Price => Region.Price(Soldiers);
		public int Score => Region.Score;
		public int Earnings => Region.Income;

		public Soldiers MaxMovable(Provinces provinces, Province to) => Soldiers.MaxMovable(provinces, this, to);
		public int AttackPower => Soldiers.AttackPower;
		public int DefensePower => Soldiers.DefensePower;
		public int Power => Soldiers.Power;
		public int DefaultDefensePower => DefaultSoldiers.DefensePower;

		public Province Subtract(Soldiers army) => WithSoldiers(Soldiers.Subtract(army));
		public Province Reinforce(Soldiers another) => WithSoldiers(Soldiers.Add(another));
		public Province AttackedBy(Player p, Soldiers s) => this with { Player = s.AttackPower > Soldiers.DefensePower ? p : Player, Soldiers = Soldiers.AttackedBy(s) };
		public Province VisitedBy(Player p, Soldiers s) => IsAllyOf(p) ? Reinforce(s) : AttackedBy(p, s);

		public bool IsAllyOf(Player p) => p == Player;
		public bool IsAllyOf(Province prov) => prov.Player == Player;
		public bool IsEnemyOf(Player p) => Inhabited && p != Player;

		public bool Has(Soldiers soldiers) => Soldiers.Contains(soldiers);
		public bool HasSoldiers => Soldiers.Any;
		public bool CanPersist => Soldiers.CanSurviveIn(Region);
		public bool CanPersistWithout(Soldiers s) => Subtract(s).CanPersist;
		public bool CanAnyMove(Provinces provinces, Province to) => Soldiers.Any(reg => reg.CanMoveAlone(provinces, this, to));
		public bool CanMove(Provinces provinces, Province to, Player player, Soldiers soldiers) => IsAllyOf(player) && soldiers.CanMove(provinces, this, to) && CanPersistWithout(soldiers);

		public Color Fill => Player.ColorOf(Player).Over(Region.Fill(Settings));
		public Color Stroke => Region.Stroke(Settings);
		public double StrokeWidth => Region.StrokeWidth(Settings);

		public bool Equals(Province? other) => other?.Region == Region;
		public override int GetHashCode() => Region.GetHashCode();
	}
}