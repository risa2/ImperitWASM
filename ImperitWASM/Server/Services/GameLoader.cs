using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IGameLoader
	{
		Game? this[int gameId] { get; set; }
		int Add(Game added);
		void RemoveOld(DateTimeOffset deadline);
		ImmutableArray<int> ShouldStart { get; }
		int? RegistrableGame { get; }
	}
	public class GameLoader : IGameLoader
	{
		readonly IDatabase db;
		public GameLoader(IDatabase database) => db = database;

		public Game? this[int gameId]
		{
			get
			{
				var result = db.Query<int, long, long>("SELECT CurrentState, StartTime, FinishTime FROM Game WHERE Id=@0", gameId).ToArray();
				return result.Length == 0 ? null : new Game((Game.State)result[0].Item1, DateTimeOffset.FromUnixTimeSeconds(result[0].Item2), DateTimeOffset.FromUnixTimeSeconds(result[0].Item3));
			}
			set => db.Command("UPDATE Game SET CurrentState=@1, StartTime=@2, FinishTime=@3 WHERE Id=@0", gameId, (int)value!.Current, value!.StartTime.ToUnixTimeSeconds(), value!.FinishTime.ToUnixTimeSeconds());
		}
		public ImmutableArray<int> ShouldStart => db.Query<int>("SELECT Id FROM Game WHERE CurrentState=@0 AND StartTime <= @1", (int)Game.State.Countdown, DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToImmutableArray();
		public int? RegistrableGame => db.Query<int?>("SELECT Id FROM Game WHERE CurrentState=@0 OR CurrentState=@1 LIMIT 1", (int)Game.State.Created, (int)Game.State.Countdown).FirstOrDefault();
		public int Add(Game g)
		{
			db.Command("INSERT INTO Game (CurrentState,StartTime,FinishTime) VALUES (@0,@1,@2)", (int)g.Current, g.StartTime.ToUnixTimeSeconds(), g.FinishTime.ToUnixTimeSeconds());
			return db.Query<int>("SELECT last_insert_rowid()").First();
		}
		public void RemoveOld(DateTimeOffset deadline) => db.Command("DELETE FROM Game WHERE FinishTime < @0", deadline.ToUnixTimeSeconds());
	}
}
