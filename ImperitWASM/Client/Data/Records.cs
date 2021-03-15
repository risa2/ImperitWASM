using System;
using System.Collections.Immutable;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Client.Data
{
	public sealed record Click(int Player, int? From, int Clicked, int Game);
	public sealed record DonationCmd(int P, string Key, int Recipient, int Amount, int Game);
	public sealed record GameInfo(Game.State S = Game.State.Invalid, bool A = false);
	public sealed record HistoryRecord(ImmutableArray<Powers> Powers, ImmutableArray<Color> Colors, string Name, Color Color);
	public sealed record MoveCmd(int P, string Key, int From, int To, ImmutableArray<int> Counts, int Game);
	public sealed record MoveData(int F, int T, int G);
	public enum MoveErrors { Ok, FewSoldiers, LittleCapacity, NotPlaying, Else }
	public sealed record MoveInfo(bool Possible, string FromName, string ToName, string FromSoldiers, string ToSoldiers, ImmutableArray<int> Counts);
	public sealed record PlayerId(int P, int G);
	public sealed record PlayerInfo(int P, string N, Color C, bool A, int M, int Debt, int I);
	public sealed record ProvinceDisplay(ImmutableArray<Point> B, Point C, Color F, Color S, int W, ImmutableArray<string> T)
	{
		public Color GetColor() => F.Light() > 180 ? new Color(0, 0, 0) : new Color(255, 255, 255);
		public ProvinceDisplay Update(ProvinceUpdate u) => this with { F = u.F, T = u.T };
	}
	public sealed record ProvinceUpdate(ImmutableArray<string> T, Color F);
	public sealed record PurchaseCmd(int P, string Key, int Land, int Game);
	public sealed record PurchaseData(int P, int L, int G);
	public sealed record PurchaseInfo(bool Possible = false, string Name = "", int Price = 0, int Money = 0);
	public sealed record RecruitCmd(int P, string Key, int Province, ImmutableArray<int> Counts, int Game);
	public sealed record RecruitData(int W, int P, int G);
	public sealed record RecruitInfo(string N, string S, ImmutableArray<bool> R, int M, Ratio I);
	public sealed record RegisteredPlayer(string N, string P, int S, int G);
	public enum RegistrationErrors { Ok, UsedName, NoName, InvalidStart, NoPassword }
	public sealed record SoldiersItem(Description D, int P);
	public sealed record StartInfo(Game.State S = Game.State.Invalid, DateTimeOffset D = default);
	public sealed record Switch(int? Select = null, View View = View.Map, int? From = null, int? To = null);
	public enum View { Map, Donation, Move, Preview, Purchase, Recruit, Statistics }
	public sealed record Winner(string N, Color C);
}
