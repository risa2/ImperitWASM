using System;
using System.Linq;
using System.Security.Cryptography;

namespace ImperitWASM.Server.Services
{
	public interface ISessionLoader
	{
		string Add(string playerName, bool fromTransaction);
		bool IsValid(string player, string key);
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
		public string Add(string player, bool fromTransaction)
		{
			string key = GenerateToken(64);
			db.Transaction(!fromTransaction, () =>
			{
				while (db.Query<int>("SELECT Count(Key) FROM Session WHERE Key = @x0", key).First() > 0)
				{
					key = GenerateToken(64);
				}
				db.Command("INSERT INTO Session (Key, PlayerName) VALUES (@x0, @x1)", key, player);
			});
			return key;
		}
		public bool IsValid(string player, string key) => db.Query<int>("SELECT Count(Key) FROM Session WHERE Key=@x0 AND PlayerName=@x1", key, player).First() > 0;
		public void Remove(string key) => db.Command("DELETE FROM Session WHERE Key=@x0", key);
	}
}
