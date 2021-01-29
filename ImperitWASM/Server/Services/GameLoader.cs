using System;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IGameLoader
	{
		Game this[int gameId] { get; set; }
		void Add(Game added);
		void RemoveOld(DateTimeOffset deadline);
	}
	public class GameLoader : IGameLoader
	{
		readonly IDatabase db;
		public GameLoader(IDatabase database) => db = database;

		public Game this[int gameId]
		{
			get
			{
				var result = db.Query<int, long, long>("SELECT CurrentState, StartTime, FinishTime FROM Game WHERE Id=@x0", gameId).First();
				return new Game((Game.State)result.Item1, DateTimeOffset.FromUnixTimeSeconds(result.Item2), DateTimeOffset.FromUnixTimeSeconds(result.Item3));
			}
			set => db.Command("UPDATE Game SET CurrentState=@x1, StartTime=@x2, FinishTime=@x3 WHERE Id=@x0", gameId, (int)value.Current, value.StartTime.ToUnixTimeSeconds(), value.FinishTime.ToUnixTimeSeconds());
		}

		public void Add(Game g) => db.Command("INSERT INTO Game (CurrentState,StartTime,FinishTime) VALUES (@x0,@x1,@x2)", (int)g.Current, g.StartTime.ToUnixTimeSeconds(), g.FinishTime.ToUnixTimeSeconds());
		public void RemoveOld(DateTimeOffset deadline) => db.Command("DELETE FROM Game WHERE FinishTime < @x0", deadline.ToUnixTimeSeconds());
	}
}
