using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class JsonPlayerAction : IEntity<IPlayerAction, Settings>
	{
		public string? Type { get; set; }
		public int? Debt { get; set; }
		public IPlayerAction Convert(int i, Settings settings) => Type switch
		{
			"Default" => new Default(),
			"Instability" => new Instability(),
			"Loan" => new Loan(Debt ?? 0, settings),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static JsonPlayerAction From(IPlayerAction action)
		{
			return action switch
			{
				Default _ => new JsonPlayerAction { Type = "Default" },
				Instability _ => new JsonPlayerAction { Type = "Instability" },
				Loan Loan => new JsonPlayerAction { Type = "Loan", Debt = Loan.Debt },
				_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
			};
		}
	}
}