using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ImperitWASM.Shared.Cfg
{
	public class Password : IEquatable<Password>
	{
		static readonly SHA256 sha = SHA256.Create();
		public byte[] Hash { get; }
		public Password(byte[] hash) => Hash = hash;
		public Password(string? str) => Hash = sha.ComputeHash(Encoding.UTF8.GetBytes(str ?? string.Empty));
		public static Password Parse(string? b64) => new Password(Convert.FromBase64String(b64 ?? string.Empty));
		public bool Equals(Password? other) => other is Password && Hash.SequenceEqual(other.Hash);
		public bool IsCorrect(string? other) => Equals(new Password(other));
		public override bool Equals(object? o) => Equals(o as Password);
		public static bool operator ==(Password? x, Password? y) => ReferenceEquals(x, y) || (x is Password && x.Equals(y));
		public static bool operator !=(Password? x, Password? y) => !(x == y);
		public override int GetHashCode() => Hash.GetHashCode();
		public override string ToString() => Convert.ToBase64String(Hash);
	}
}