using System.Collections.Generic;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IPowers
	{
		List<PlayersPower> Get(int gameId);
		int Count(int gameId);
		void Add(int gameId, PlayersAndProvinces pap);
	}
	public class Powers : IPowers
	{
		readonly IContextService ctx;
		public Powers(IContextService ctx) => this.ctx = ctx;
		public List<PlayersPower> Get(int gameId) => ctx.GetPlayersPowers(gameId);
		public void Add(int gameId, PlayersAndProvinces pap) => ctx.Add(gameId, pap.PlayersPower(p => p is not Savage));
		public int Count(int gameId) => ctx.CountPlayersPowers(gameId);
	}
}