using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public sealed record PlayerIdentity(string Name, int Order, int GameId, bool Human)
	{
		public static Color ColorOf(int order) => Color.Generate(order, 120.0, 1.0, 1.0);
		public Color Color => ColorOf(Order);
		public static Color ColorOf(PlayerIdentity? ip) => ip?.Color ?? new Color();

		public bool Equals(PlayerIdentity? ip) => ip is not null && Order == ip.Order && GameId == ip.GameId;
		public override int GetHashCode() => (Order, GameId).GetHashCode();
	}
}
