using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayerAction : IEntity
	{
		public enum Kind { EndTurn, Loan }
		[Key] public int Id { get; set; }
		public EntityPlayer? EntityPlayer { get; set; }
		public int EntityPlayerId { get; set; }
		public Kind Type { get; set; }
		public int? Debt { get; set; }
		public IPlayerAction Convert(Settings settings) => Type switch
		{
			Kind.Loan => new Loan(Debt ?? 0, settings),
			_ => new EndTurn()
		};
		public static EntityPlayerAction From(IPlayerAction action) => action switch
		{
			Loan Loan => new EntityPlayerAction { Type = Kind.Loan, Debt = Loan.Debt },
			_ => new EntityPlayerAction { Type = Kind.EndTurn }
		};
	}
}