using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ImperitWASM.Shared.Data
{
	public readonly struct Password
	{
		static string PasswordHash(string password)
		{
			byte[] salt = new byte[16];
			new RNGCryptoServiceProvider().GetBytes(salt);
			byte[] hash = new Rfc2898DeriveBytes(password, salt, 100000).GetBytes(20);
			byte[] result = new byte[36];
			Array.Copy(salt, 0, result, 0, 16);
			Array.Copy(hash, 0, result, 16, 20);
			return Convert.ToBase64String(result);
		}

		static bool PasswordVerify(string password, string hashString)
		{
			byte[] hash = Convert.FromBase64String(hashString);
			byte[] hash2 = new Rfc2898DeriveBytes(password, hash[0..16], 100000).GetBytes(20);
			for (int i = 0; i < 20; ++i)
			{
				if (hash[i + 16] != hash2[i])
				{
					return false;
				}
			}
			return true;
		}

		readonly string hash;
		Password(string hash) => this.hash = hash;
		public static Password FromHash(string hash) => new Password(hash);
		public static Password FromPassword(string pw) => new Password(PasswordHash(pw));
		public bool IsCorrect(string other) => other is not null && PasswordVerify(other, hash);
		public override int GetHashCode() => hash.GetHashCode();
		public override string ToString() => hash;
	}
}