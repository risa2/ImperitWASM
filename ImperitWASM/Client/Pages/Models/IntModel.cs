namespace ImperitWASM.Client.Pages
{
	public class IntModel
	{
		public int Value { get; set; } = 0;
		public static IntModel[] Array(int count)
		{
			var arr = new IntModel[count];
			for (int i = 0; i < count; ++i)
			{
				arr[i] = new IntModel();
			}
			return arr;
		}
	}
}
