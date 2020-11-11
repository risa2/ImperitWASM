using System;
using System.ComponentModel.DataAnnotations;

namespace ImperitWASM.Server.Load
{
	public class EntityGame : IEntity
	{
		public enum State { Created, Countdown, Started, Finished }
		[Key] public int Id { get; set; }
		public int Active { get; set; }
		public State Current { get; set; }
		public DateTime LastChange { get; set; }
		public bool Created => Current == State.Created;
		public bool Countdown => Current == State.Countdown;
		public bool Started => Current == State.Started;
		public bool Finished => Current == State.Finished;
		public static EntityGame Create => new EntityGame { Current = State.Created, LastChange = DateTime.UtcNow };
		public EntityGame StartCountdown()
		{
			LastChange = Countdown ? LastChange : DateTime.UtcNow;
			Current = State.Countdown;
			return this;
		}
		public EntityGame Start()
		{
			LastChange = Started ? LastChange : DateTime.UtcNow;
			Current = State.Started;
			return this;
		}
		public EntityGame Finish()
		{
			LastChange = Finished ? LastChange : DateTime.UtcNow;
			Current = State.Started;
			return this;
		}
		public EntityGame SetActive(int i)
		{
			Active = i;
			return this;
		}
	}
}
