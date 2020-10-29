using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayerAction : IEntity<IPlayerAction, Settings>
	{
		[Key] public int Id { get; set; }
		public string Type { get; set; } = "";
		public int? Debt { get; set; }
		public IPlayerAction Convert(Settings settings) => Type switch
		{
			"Default" => new Default(),
			"Instability" => new Instability(),
			"Loan" => new Loan(Debt ?? 0, settings),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static EntityPlayerAction From(IPlayerAction action)
		{
			return action switch
			{
				Default _ => new EntityPlayerAction { Type = "Default" },
				Instability _ => new EntityPlayerAction { Type = "Instability" },
				Loan Loan => new EntityPlayerAction { Type = "Loan", Debt = Loan.Debt },
				_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
			};
		}
	}
}