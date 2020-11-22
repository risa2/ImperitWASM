using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayerAction : IEntity
	{
		public enum Kind { Default, Instability, Loan }
		[Key] public int Id { get; set; }
		public EntityPlayer? EntityPlayer { get; set; }
		public int EntityPlayerId { get; set; }
		public Kind Type { get; set; }
		public int? Debt { get; set; }
		public IPlayerAction Convert(Settings settings) => Type switch
		{
			Kind.Loan => new Loan(Debt ?? 0, settings),
			Kind.Instability => new Instability(),
			_ => new Default(),
		};
		public static EntityPlayerAction From(IPlayerAction action) => action switch
		{
			Loan Loan => new EntityPlayerAction { Type = Kind.Loan, Debt = Loan.Debt },
			Instability _ => new EntityPlayerAction { Type = Kind.Instability },
			_ => new EntityPlayerAction { Type = Kind.Default }
		};
	}
}