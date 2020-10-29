using System.Collections.Generic;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IPlayersProvinces
	{
		Player Player(int i);
		Province Province(int i);
		int PlayersCount { get; }
		int ProvincesCount { get; }
		int LivingHumans { get; }
		Task Save();
		void RemovePlayers();
		void Add(Player player);
		void Add(Player player, Soldiers soldiers, int start);
		void Add(IEnumerable<(Player, Soldiers, int)> starts);
		bool Do(ICommand cmd);
		void Next();
		void ResetActive();
		IEnumerable<Player> Players { get; }
		IEnumerable<Province> Provinces { get; }
		PlayersAndProvinces PaP { get; set; }
		Player Active { get; }
	}
	public class PlayersProvinces : IPlayersProvinces
	{
		readonly PlayersAndProvincesLoader loader;
		public PlayersAndProvinces PaP { get; set; }
		public PlayersProvinces(IServiceIO io, ISettingsLoader sl)
		{
			loader = new PlayersAndProvincesLoader(io.Provinces, io.Graph, io.Shapes, io.Players, io.Active, sl.Settings);
			PaP = loader.Load();
		}
		public int PlayersCount => PaP.PlayersCount;
		public int ProvincesCount => PaP.ProvincesCount;
		public Player Player(int i) => i >= 0 && i < PlayersCount ? PaP.Player(i) : new Savage(i);
		public Province Province(int i) => PaP.Province(i);
		public Task Save() => loader.Save(PaP);
		public void Add(IEnumerable<(Player, Soldiers, int)> starts) => PaP = PaP.Add(starts);
		public void Add(Player player, Soldiers soldiers, int start) => Add(new[] { (player, soldiers, start) });
		public void Add(Player player) => PaP = PaP.Add(player);
		public void RemovePlayers() => PaP = PaP.RemovePlayers();
		public bool Do(ICommand cmd)
		{
			var (new_pap, success) = PaP.Do(cmd);
			PaP = new_pap;
			return success;
		}
		public void Next() => PaP = PaP.Do(new Shared.Motion.Commands.NextTurn()).Item1.Act().Next();
		public void ResetActive() => PaP = PaP.ResetActive();
		public int LivingHumans => PaP.LivingHumans;
		public IEnumerable<Player> Players => PaP.Players;
		public IEnumerable<Province> Provinces => PaP.Provinces;
		public Player Active => PaP.Active;
	}
}