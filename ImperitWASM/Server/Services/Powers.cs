using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IPowers
	{
		List<PlayersPower> Get(int gameId);
		ImmutableDictionary<int, ImmutableArray<PlayersPower>> All { get; }
		int Count(int gameId);
		void Add(int gameId);
	}
	public class Powers : IPowers
	{
		readonly IPlayersProvinces pap;
		readonly IContextService ctx;

		public Powers(IPlayersProvinces pap, IContextService ctx)
		{
			this.pap = pap;
			this.ctx = ctx;
		}
		public List<PlayersPower> Get(int gameId) => ctx.GetPlayersPowers(gameId);
		public void Add(int gameId) => ctx.AddPlayersPower(gameId, PlayersPower.Compute(pap[gameId]));
		public int Count(int gameId) => ctx.CountPlayersPowers(gameId);
		public ImmutableDictionary<int, ImmutableArray<PlayersPower>> All => ctx.PlayerPowers.GroupBy(p => p.EntityGameId).ToImmutableDictionary(pps => pps.Key, pps => EntityPlayerPower.ConvertMore(pps));
	}
}