using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Data
{
	public class PlayerInfo
	{
		public PlayerInfo(bool isActive, Color color)
		{
			IsActive = isActive;
			Color = color;
		}
		public PlayerInfo() { }
		public bool IsActive { get; set; }
		public Color Color { get; set; }
	}
}
