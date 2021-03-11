using System;
using System.Linq;
using System.Security.Cryptography;

namespace ImperitWASM.Server.Services
{
	public interface ISessionLoader
	{
		string Add(int player, int gameId, bool fromTransaction);
		bool IsValid(int player, int gameId, string key);
		void Remove(string key);
	}
	public class SessionLoader : ISessionLoader
	{
		static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
		readonly IDatabase db;
		public SessionLoader(IDatabase db) => this.db = db;
		static string GenerateToken(int len)
		{
			byte[] buf = new byte[len];
			rng.GetBytes(buf);
			return Convert.ToBase64String(buf).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}
		public string Add(int player, int gameId, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			string key = GenerateToken(64);
			while (db.Query<int>("SELECT Count(Key) FROM Session WHERE Key = @0", key).First() > 0)
			{
				key = GenerateToken(64);
			}
			db.Command("INSERT INTO Session (Key,GameId,Player) VALUES (@0,@1,@2)", key, gameId, player);
			return key;
		});
		public bool IsValid(int player, int gameId, string key) => db.Query<int>("SELECT Count(Key) FROM Session WHERE Key=@0 AND GameId=@1 AND Player=@2", key, gameId, player).First() > 0;
		public void Remove(string key) => db.Command("DELETE FROM Session WHERE Key=@0", key);
	}
}
