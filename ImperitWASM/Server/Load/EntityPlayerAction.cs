using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayerAction : IEntity
	{
		[Key] public int Id { get; set; }
		public EntityPlayer? EntityPlayer { get; set; }
		public int EntityPlayerId { get; set; }
		public string Type { get; set; } = "";
		public int? Debt { get; set; }
		public IPlayerAction Convert(Settings settings) => Type switch
		{
			"D" => new Default(),
			"I" => new Instability(),
			"L" => new Loan(Debt ?? 0, settings),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static EntityPlayerAction From(IPlayerAction action) => action switch
		{
			Default _ => new EntityPlayerAction { Type = "D" },
			Instability _ => new EntityPlayerAction { Type = "I" },
			Loan Loan => new EntityPlayerAction { Type = "L", Debt = Loan.Debt },
			_ => throw new System.Exception("Unknown type of Action: " + action)
		};
	}
}