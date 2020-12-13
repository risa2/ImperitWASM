using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Load
{
	public class Game : IEntity
	{
		public enum State { Created, Countdown, Started, Finished }
		[Key] public int Id { get; set; }
		public int Active { get; set; }
		public State Current { get; set; }
		public DateTime LastChange { get; set; }
		public ICollection<EntityPlayer>? EntityPlayers { get; set; }
		public ICollection<EntityProvince>? EntityProvinces { get; set; }
		public ICollection<EntitySession>? EntitySessions { get; set; }
		public ICollection<EntityPlayerPower>? EntityPlayerPowers { get; set; }
		public bool Started => Current == State.Started;
		public static Game Create => new Game { Current = State.Created, LastChange = DateTime.UtcNow };
		public Game StartCountdown()
		{
			LastChange = DateTime.UtcNow;
			Current = State.Countdown;
			return this;
		}
		public Game Start()
		{
			LastChange = DateTime.UtcNow;
			Current = State.Started;
			return this;
		}
		public Game Finish()
		{
			LastChange = DateTime.UtcNow;
			Current = State.Finished;
			return this;
		}
		public Game SetActive(int i)
		{
			Active = i;
			return this;
		}
		public ImmutableArray<Player> GetPlayers(Settings set) => EntityPlayers!.OrderBy(p => p.Index).Select(p => p.Convert(set)).ToImmutableArray();
		public ImmutableArray<Province> GetProvinces(Settings set) => EntityProvinces!.OrderBy(p => p.Index).Select(p => p.Convert(set)).ToImmutableArray();
	}
}
