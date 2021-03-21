using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public sealed record Province(Region Region, PlayerIdentity? Ruler, Soldiers Soldiers, Settings Settings)
	{
		public int Order => Region.Order;
		public string Name => Region.Name;
		public ImmutableArray<string> Text => Region.Text(Soldiers);
		public ImmutableArray<Point> Border => Region.Border;
		public Point Center => Region.Center;
		public Soldiers DefaultSoldiers => Region.Soldiers;

		public Province RuledBy(PlayerIdentity ruler) => this with { Ruler = ruler };
		public Province WithSoldiers(Soldiers soldiers) => this with { Soldiers = soldiers };
		public Province Revolt() => this with { Ruler = null, Soldiers = DefaultSoldiers };

		public bool Recruitable(SoldierType type) => Region.Recruitable(type);
		public Ratio Instability => Region.Instability(Soldiers);
		public bool Unstable => !CanPersist || Instability.Any;
		public bool Shaky(PlayerIdentity active) => !CanPersist || (IsAllyOf(active) && Instability.RandomBool);
		public Province RevoltIfShaky(PlayerIdentity active) => Shaky(active) ? Revolt() : this;
		public bool KeepsPlayerAlive => HasSoldiers || Mainland;

		public bool Inhabited => Ruler is not null;
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
		public Province AttackedBy(PlayerIdentity ip, Soldiers s) => this with { Ruler = s.AttackPower > Soldiers.DefensePower ? ip : Ruler, Soldiers = Soldiers.AttackedBy(s) };
		public Province VisitedBy(PlayerIdentity ip, Soldiers s) => IsAllyOf(ip) ? Reinforce(s) : AttackedBy(ip, s);

		public bool IsAllyOf(PlayerIdentity ip) => ip == Ruler;
		public bool IsAllyOf(Province prov) => prov.Ruler == Ruler;
		public bool IsEnemyOf(PlayerIdentity ip) => Inhabited && ip != Ruler;

		public bool Has(Soldiers soldiers) => Soldiers.Contains(soldiers);
		public bool HasSoldiers => Soldiers.Any;
		public bool HasShip => Soldiers.Any(r => r.Type is Ship);
		public bool CanPersist => Soldiers.CanSurviveIn(Region);
		public bool CanPersistWithout(Soldiers s) => Subtract(s).CanPersist;
		public bool CanAnyMove(Provinces provinces, Province to) => Soldiers.Any(reg => reg.CanMoveAlone(provinces, this, to));
		public bool CanMove(Provinces provinces, Province to, PlayerIdentity ip, Soldiers soldiers) => IsAllyOf(ip) && soldiers.CanMove(provinces, this, to) && CanPersistWithout(soldiers);

		public Color Fill => Region.Fill(Settings, PlayerIdentity.ColorOf(Ruler));
		public Color Stroke => Region.Stroke(Settings);
		public int StrokeWidth => Region.StrokeWidth(Settings);

		public bool Equals(Province? other) => other?.Region == Region;
		public override int GetHashCode() => Region.GetHashCode();
	}
}