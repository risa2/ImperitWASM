namespace ImperitWASM.Client.Pages
{
	public class MoveModel
	{
		public enum Type { Attack, Reinforcement }
		public IntModel[] Soldiers = System.Array.Empty<IntModel>();
		public Type MoveType { get; set; }
		public bool IsAttack
		{
			get => MoveType == Type.Attack;
			set => MoveType = value ? Type.Attack : Type.Reinforcement;
		}
	}
}
