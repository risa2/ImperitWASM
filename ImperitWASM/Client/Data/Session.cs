using System.Text.Json.Serialization;

namespace ImperitWASM.Client.Data
{
	public sealed record Session
	{
		[JsonInclude] public int G { get; private set; }
		[JsonInclude] public int P { get; private set; }
		[JsonInclude] public string Key { get; private set; }
		public bool IsSet() => Key is { Length: > 0 };
		public Session(int g, int p, string key) => (G, P, Key) = (g, p, key);
		public Session() : this(0, 0, "") { }
	}
}
