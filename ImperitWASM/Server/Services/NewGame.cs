using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace ImperitWASM.Server.Services
{
	public interface INewGame
	{
		Task Finish();
		Task Start();
		Color NextColor { get; }
		Task Registration(string name, Password password, Land land);
	}
	public class NewGame : INewGame
	{
		static readonly Random rand = new Random();
		readonly ISettingsLoader sl;
		readonly IPlayersProvinces pap;
		readonly IPowersLoader powers;
		readonly ILoginService login;
		readonly IGameLoader game;
		readonly IFormerPlayers former;
		readonly static ImmutableList<IPlayerAction> Actions = ImmutableList.Create<IPlayerAction>(new Default(), new Instability());
		public NewGame(ISettingsLoader sl, IPlayersProvinces pap, IPowersLoader powers, ILoginService login, IGameLoader game, IFormerPlayers former)
		{
			this.sl = sl;
			this.pap = pap;
			this.powers = powers;
			this.login = login;
			this.game = game;
			this.former = former;
		}
		public async Task Finish()
		{
			former.Reset(pap.Players);
			await login.Reset(0);
			await game.Finish();
			
			pap.RemovePlayers();
			pap.Add(new Savage(0));
			await pap.Save();
		}
		public Color NextColorFn(int i) => new Color(120.0 + (137.507764050037854 * (pap.PlayersCount - 1 + i)), 1.0, 1.0);
		public Color NextColor => NextColorFn(0);
		Province[] UnoccupiedStartLands(int max) => pap.Provinces.Where(p => p is Land L1 && L1.IsStart && !L1.Occupied).Take(max).ToArray();
		Robot GetRobot(int earnings, int i)
		{
			return new Robot(pap.PlayersCount + i, sl.Settings.RobotNames[i], NextColorFn(i), new Password(""), sl.Settings.DefaultMoney - (earnings * 2), true, sl.Settings, Actions);
		}
		void AddRobots()
		{
			var starts = UnoccupiedStartLands(sl.Settings.RobotNames.Length);
			rand.Shuffle(starts);
			pap.Add(starts.Select((start, i) => (GetRobot(start.Earnings, i) as Player, start.Soldiers, start.Id)));
		}
		public async Task Start()
		{
			AddRobots();
			await powers.Clear();
			await game.Start();
			
			powers.Compute();
			pap.ResetActive();
			await pap.Save();
			await login.Reset(pap.PlayersCount);
		}
		public async Task Registration(string name, Password password, Land land)
		{
			var player = new Player(pap.PlayersCount, name, NextColor, password, sl.Settings.DefaultMoney - (land.Earnings * 2), true, Actions);
			pap.Add(player, land.DefaultSoldiers, land.Id);
			await pap.Save();
			await game.Register();
		}
	}
}