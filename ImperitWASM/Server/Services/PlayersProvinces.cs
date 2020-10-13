using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Server.Services
{
	public interface IPlayersProvinces
	{
		Player Player(int i);
		Province Province(int i);
		int PlayersCount { get; }
		int ProvincesCount { get; }
		int LivingHumans { get; }
		void Save();
		void RemovePlayers();
		void Add(Player player);
		void Add(Player player, Soldiers soldiers, int start);
		void Add(IEnumerable<(Player, Soldiers, int)> starts);
		bool Do(ICommand cmd);
		void Act();
		void Next();
		IReadOnlyList<Player> Players { get; }
		Provinces Provinces { get; }
		PlayersAndProvinces PaP { get; }
		Player Active { get; }
	}
	public class PlayersProvinces : IPlayersProvinces
	{
		readonly PlayersAndProvincesLoader loader;
		PlayersAndProvinces pap;
		public PlayersProvinces(IServiceIO io, ISettingsLoader sl)
		{
			loader = new PlayersAndProvincesLoader(io.Provinces, io.Graph, io.Shapes, io.Players, io.Active, sl.Settings);
			pap = loader.Load();
		}
		public int PlayersCount => pap.PlayersCount;
		public int ProvincesCount => pap.ProvincesCount;
		public Player Player(int i) => pap.Player(i);
		public Province Province(int i) => pap.Province(i);
		public void Save() => loader.Save(pap);
		public void Add(IEnumerable<(Player, Soldiers, int)> starts) => pap = pap.Add(starts);
		public void Add(Player player, Soldiers soldiers, int start) => Add(new[] { (player, soldiers, start) });
		public void Add(Player player) => pap = pap.Add(player);
		public void RemovePlayers() => pap = pap.RemovePlayers();
		public bool Do(ICommand cmd)
		{
			var (new_pap, success) = pap.Do(cmd);
			pap = new_pap;
			return success;
		}
		public void Act() => pap = pap.Act();
		public void Next() => pap = pap.Next();
		public int LivingHumans => pap.LivingHumans;
		public IReadOnlyList<Player> Players => pap.Players;
		public Provinces Provinces => pap.Provinces;
		public Player Active => pap.Active;
		public PlayersAndProvinces PaP => pap;
	}
}