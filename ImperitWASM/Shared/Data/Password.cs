using System;
using System.Linq;
using System.Security.Cryptography;

namespace ImperitWASM.Shared.Data
{
	public readonly struct Password
	{
		const int SaltBytes = 16, HashBytes = 20;
		static byte[] RandomBytes(int count)
		{
			byte[] result = new byte[count];
			new RNGCryptoServiceProvider().GetBytes(result);
			return result;
		}
		static string PasswordHash(string password)
		{
			byte[] salt = RandomBytes(SaltBytes);
			byte[] hash = new Rfc2898DeriveBytes(password, salt, 100000).GetBytes(20);
			byte[] result = new byte[SaltBytes + HashBytes];
			Array.Copy(salt, 0, result, 0, SaltBytes);
			Array.Copy(hash, 0, result, SaltBytes, HashBytes);
			return Convert.ToBase64String(result);
		}

		static bool PasswordVerify(string password, string hashString)
		{
			byte[] hash = Convert.FromBase64String(hashString);
			byte[] hash2 = new Rfc2898DeriveBytes(password, hash[0..SaltBytes], 100000).GetBytes(20);
			return hash.Skip(SaltBytes).SequenceEqual(hash2);
		}

		readonly string hash;
		Password(string hash) => this.hash = hash;
		public static Password FromHash(string hash) => new Password(hash);
		public static Password FromPassword(string pw) => new Password(PasswordHash(pw));
		public static Password Random => new Password(Convert.ToBase64String(RandomBytes(SaltBytes + HashBytes)));
		public bool IsCorrect(string other) => other is not null && PasswordVerify(other, hash);
		public override int GetHashCode() => hash.GetHashCode();
		public override string ToString() => hash;
	}
}