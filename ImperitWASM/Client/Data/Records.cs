using System.Collections.Immutable;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Client.Data
{
	public sealed record Click(int Player, int? From, int Clicked, int Game);
	public record ColoredPlayer(string N, Color C);
	public sealed record DonationCmd(int P, string Key, int Recipient, int Amount, int Game);
	public sealed record GameInfo(GameState S = GameState.Invalid, int P = 0);
	public enum GameState { Created, Countdown, Started, Finished, Invalid = -1 }
	public sealed record MoveCmd(int P, string Key, int From, int To, ImmutableArray<int> Counts, int Game);
	public sealed record MoveData(int F, int T, int G);
	public enum MoveErrors { Ok, FewSoldiers, LittleCapacity, NotPlaying, Else }
	public sealed record MoveInfo(bool Possible, bool CanAttack, bool CanReinforce, string FromName, string ToName, string FromSoldiers, string ToSoldiers, ImmutableArray<Description> Soldiers);
	public sealed record PlayerId(int P, int G);
	public sealed record PlayerInfo(bool D, string N, Color C, bool A, int M, int Debt, int I) : ColoredPlayer(N, C);
	public sealed record ProvinceAppearance(ImmutableArray<Point> B, Point C, Color F, Color S, int W, bool R, ImmutableArray<string> T)
	{
		public Color GetColor() => F.Light() > 180 ? new Color(0, 0, 0) : new Color(255, 255, 255);
		public Color GetRegistrationColor() => R ? GetColor() : new Color(80, 80, 80);
		public ProvinceAppearance Update(Color f, ImmutableArray<string> t) => this with { F = f, T = t };
	}
	public sealed record ProvinceUpdate(ImmutableArray<string> T, Color F);
	public sealed record PurchaseCmd(int P, string Key, int Land, int Game);
	public sealed record PurchaseData(int P, int L, int G);
	public sealed record PurchaseInfo(bool Possible, string Name, int Price, int Money);
	public sealed record RecruitCmd(int P, string Key, int Province, ImmutableArray<int> Counts, int Game);
	public sealed record RecruitData(int W, int P, int G);
	public sealed record RecruitInfo(string N, string S, ImmutableArray<SoldiersItem> R, int M, Ratio I);
	public sealed record RegisteredPlayer(string N, string P, int S, int G);
	public enum RegistrationErrors { Ok, UsedName, NoName, InvalidStart, NoPassword }
	public sealed record SoldiersItem(Description D, int P);
	public sealed record Session(int P = 0, int G = 0, string Key = "")
	{
		public bool IsSet() => Key.Length > 0;
	}
	public sealed record Switch(int? Select, View View, int? From, int? To);
	public enum View { Map, Donation, Move, Preview, Purchase, Recruit, Statistics }
}
