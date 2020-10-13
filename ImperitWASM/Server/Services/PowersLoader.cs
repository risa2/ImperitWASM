using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Server.Services
{
	public interface IPowersLoader : IReadOnlyList<PlayersPower>
	{
		void Compute();
		void Clear();
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
		public void Clear()
		{
			loader.Clear();
			powers.Clear();
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