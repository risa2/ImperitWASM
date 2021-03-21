using System.Collections.Immutable;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Region(int Order, string Name, Shape Shape, Soldiers Soldiers, ImmutableArray<SoldierType> ExtraTypes)
	{
		public int AttackPower => Soldiers.AttackPower;
		public int DefensePower => Soldiers.DefensePower;
		public int Power => Soldiers.Power;
		public ImmutableArray<Point> Border => Shape.Border;
		public Point Center => Shape.Center;

		public virtual Color Fill(Settings settings) => new Color();
		public virtual Color Stroke(Settings settings) => new Color();
		public virtual int StrokeWidth(Settings settings) => 0;

		public virtual bool Inhabitable => false;
		public virtual bool Sailable => false;
		public virtual bool Mainland => false;
		public virtual bool Dry => false;
		public virtual bool Port => false;

		public virtual int Price(Soldiers now) => int.MaxValue;
		public virtual int Score => 0;
		public virtual int Income => 0;

		public virtual Ratio Instability(Soldiers present) => Ratio.Zero;
		public virtual ImmutableArray<string> Text(Soldiers present) => ImmutableArray<string>.Empty;

		public bool Recruitable(SoldierType type) => ExtraTypes.Contains(type);

		public virtual bool Equals(Region? region) => Order == region?.Order;
		public override int GetHashCode() => Order.GetHashCode();
	}
}
