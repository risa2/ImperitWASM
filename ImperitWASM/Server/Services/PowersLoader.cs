using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IPowersLoader : IReadOnlyList<PlayersPower>
	{
		void Compute();
		Task Clear();
		PlayersPower Last { get; }
	}
	public class PowersLoader : IPowersLoader
	{
		readonly IPlayersProvinces pap;
		readonly JsonWriter<PlayersPower, PlayersPower, bool> loader;
		readonly List<PlayersPower> powers;
		public int Count => powers.Count;

		public PlayersPower Last => powers[^1];
		public PlayersPower this[int i] => powers[i];
		public PowersLoader(IServiceIO io, IPlayersProvinces pap)
		{
			loader = new JsonWriter<PlayersPower, PlayersPower, bool>(io.Powers, false, x => x);
			powers = loader.Load().ToList();
			this.pap = pap;
		}
		public Task Clear()
		{
			powers.Clear();
			return loader.Clear();
		}
		public void Compute()
		{
			var values = PlayersPower.Compute(pap.PaP);
			powers!.Add(values);
			loader.Add(values);
		}
		public IEnumerator<PlayersPower> GetEnumerator() => powers.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}