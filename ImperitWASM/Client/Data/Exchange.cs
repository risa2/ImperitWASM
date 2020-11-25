using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public record BasicInfo(bool A, Color B, bool C, int D);
	public record Click(int U, int? F, int C, int G);
	public record CmdData(int A, int B, int G);
	public record ColoredHuman(string Name, Color Color);
	public record DisplayableShape(ImmutableArray<Point> B, Point C, Color F, Color S, int W, bool R, IEnumerable<string> T)
	{
		public DisplayableShape UpdateText() => R ? this : this with { T = T.Skip(1) };
		public DisplayableShape Update(Color f, IEnumerable<string> t) => this with { F = f, T = t };
	}
	public record DonationCmd(int LoggedIn, string LoginId, int Recipient, int Amount, int Game);
	public record MoveCmd(int LoggedIn, string LoginId, int From, int To, ImmutableArray<int> Counts, int Game);
	public enum MoveErrors { Ok, FewSoldiers, LittleCapacity, NotPlaying, Else }
	public record MoveInfo(bool Possible, bool CanAttack, bool CanReinforce, string FromName, string ToName, string FromSoldiers, string ToSoldiers, ImmutableArray<Description> Soldiers);
	public record PlayerId(int I, int G, string N);
	public record PlayerInfo(int Id, bool Real, string Name, string Color, bool Alive, int Money, int Income, int Debt);
	public record ProvinceInstability(string Name, Color Color, Ratio Instability);
	public record ProvinceVariables(IEnumerable<string> T, Color F);
	public record PurchaseCmd(int LoggedIn, string LoginId, int Land, int Game);
	public record PurchaseInfo(bool Possible, string Name, int Price, int Money);
	public record RecruitCmd(int LoggedIn, string LoginId, int Province, ImmutableArray<int> Counts, int Game);
	public record RecruitInfo(string N, string S, ImmutableArray<SoldiersItem> R, int M);
	public record RegisteredPlayer(string N, string P, int S, int G);
	public record SoldiersItem(Description D, int P);
	public record StringValue(string? Value = null);
	public record Session(int U = 0, int G = 0, string I = "");
	public record Switch(int? S, View M, int? F, int? T);
	public enum View { Map, Move, Purchase, Recruit, Donation, Players, Powers, Preview }
}
