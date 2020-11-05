namespace ImperitWASM.Client.Server
{
	public class StringValue
	{
		public StringValue() => Value = null;
		public StringValue(string? value) => Value = value;
		public string? Value { get; set; }
	}
}
