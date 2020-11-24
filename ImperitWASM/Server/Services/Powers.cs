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
		void Add(int gameId, PlayersAndProvinces pap);
	}
	public class Powers : IPowers
	{
		readonly IContextService ctx;
		public Powers(IContextService ctx) => this.ctx = ctx;
		public List<PlayersPower> Get(int gameId) => ctx.GetPlayersPowers(gameId);
		public void Add(int gameId, PlayersAndProvinces pap) => ctx.Add(gameId, PlayersPower.Compute(pap));
		public int Count(int gameId) => ctx.CountPlayersPowers(gameId);
		public ImmutableDictionary<int, ImmutableArray<PlayersPower>> All => ctx.PlayerPowers.GroupBy(p => p.GameId).ToImmutableDictionary(pps => pps.Key, pps => EntityPlayerPower.ConvertMore(pps));
	}
}