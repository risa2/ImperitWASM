using System.Collections.Immutable;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public record Click(int Player, int? From, int Clicked, int Game);
	public record ColoredPlayer(string N, Color C);
	public record DonationCmd(int P, string Key, int Recipient, int Amount, int Game);
	public record GameInfo(bool S = false, int P = 0);
	public record MoveCmd(int P, string Key, int From, int To, ImmutableArray<int> Counts, int Game);
	public record MoveData(int F, int T, int G);
	public enum MoveErrors { Ok, FewSoldiers, LittleCapacity, NotPlaying, Else }
	public record MoveInfo(bool Possible, bool CanAttack, bool CanReinforce, string FromName, string ToName, string FromSoldiers, string ToSoldiers, ImmutableArray<Description> Soldiers);
	public record PlayerId(int P, int G);
	public record PlayerInfo(int P, bool D, string Name, Color C, bool Alive, int Money, int Income, int Debt);
	public record ProvinceAppearance(ImmutableArray<Point> B, Point C, Color F, Color S, int W, bool R, ImmutableArray<string> T)
	{
		public ProvinceAppearance UpdateText() => R || T.IsDefaultOrEmpty ? this : this with { T = T.RemoveAt(0) };
		public ProvinceAppearance Update(Color f, ImmutableArray<string> t) => this with { F = f, T = t };
	}
	public record ProvinceUpdate(ImmutableArray<string> T, Color F);
	public record PurchaseCmd(int P, string Key, int Land, int Game);
	public record PurchaseData(int P, int L, int G);
	public record PurchaseInfo(bool Possible, string Name, int Price, int Money);
	public record RecruitCmd(int P, string Key, int Province, ImmutableArray<int> Counts, int Game);
	public record RecruitData(int W, int P, int G);
	public record RecruitInfo(string N, string S, ImmutableArray<SoldiersItem> R, int M, Ratio I);
	public record RegisteredPlayer(string N, string P, int S, int G);
	public enum RegistrationErrors { Ok, UsedName, NoName, InvalidStart, NoPassword }
	public record SoldiersItem(Description D, int P);
	public record Session(int P = 0, int G = 0, string Key = "")
	{
		public bool IsSet() => Key.Length > 0;
	}
	public record Switch(int? Select, View View, int? From, int? To);
	public enum View { Map, Move, Purchase, Recruit, Donation, Players, Powers, Preview }
}
